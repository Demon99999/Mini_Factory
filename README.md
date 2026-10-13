# Mini Factory

Тестовое задание на позицию Unity Developer: мобильная idle-игра «Мини-фабрика» с тремя производственными машинами, апгрейдами, временным Boost, offline-прогрессом, сохранением между сессиями, аналитикой и IAP.

Проект собран на Unity 6000.3.22f1 с использованием Zenject (Extenject) для DI, UniTask для async/await, Unity IAP v5 для покупок, Newtonsoft.Json для сериализации сохранений и NUnit + Unity Test Framework для EditMode тестов. UI построен на TextMeshPro.

## Быстрый старт

Клонируйте репозиторий и откройте проект в Unity Hub, выбрав версию 6000.3.22f1. Дождитесь импорта пакетов и компиляции скриптов. Откройте сцену `Assets/_Project/Scenes/Bootstrap.unity` и нажмите Play. Игра автоматически загрузит игровую сцену `Game` через `GameStateMachine` и `SceneLoader`.

## Android build

Переключитесь на платформу Android через `File → Build Settings → Android → Switch Platform`. В `Player Settings` укажите название компании, продукта `MiniFactory`, package name `com.yourname.minifactory`, минимальный API Level 26 (Android 8.0), Target API Level — автоматически, Scripting Backend — IL2CPP, Target Architectures — только ARM64, Managed Stripping Level — Low. В `Build Settings` добавьте сцены в порядке: `0: Bootstrap.unity`, `1: Game.unity`. Нажмите Build и сохраните APK. Установите на устройство или эмулятор.

Build проверялся на  Realme14 , Android 16>`.

## Архитектура

Проект разделён на два уровня DI-контейнеров. `ProjectContext` с `GlobalInstaller` биндит глобальные сервисы, живущие всю жизнь приложения: `IAssetProvider`, `IStaticDataService`, `IPersistentProgress`, `ISaveLoadService`, `ISceneLoader`, `GameStateMachine`, `StatesFactory`. Сцена `Bootstrap` содержит `Bootstrapper`, который регистрирует состояния в `GameStateMachine` и запускает `BootstrapState`. Далее `BootstrapState` инициализирует `StaticDataService`, `LoadProgressState` загружает или создаёт `PlayerProgress`, `GameLoopState` загружает игровую сцену `Game`. В сцене `Game` работает `SceneContext` с `GameInstaller`, который биндит игровые сервисы: `IEconomyService`, `IMachineService`, `IBoostService`, `IOfflineProgressService`, `IGameLoopService`, `IAnalyticsService`, `IIAPService`, `PurchaseCoordinator`. UI-компоненты (`TopBarView`, `FactoryView`, `BuyCoinsButton`) получают зависимости через `[Inject]`.

## Ключевые системы

`GameStateMachine` управляет жизненным циклом приложения. `StaticDataService` загружает конфиги из `Resources/Configs` через `AssetProvider` и раздаёт их через абстракцию `IStaticDataService`, что позволяет позже заменить источник на Remote Config без изменения игровой логики. `PlayerProgress` — единая модель состояния игрока (баланс, машины, timestamps), сохраняется в `PlayerPrefs` через `SaveLoadService` в формате JSON. `GameLoopService` вызывается из `GameplayBootstrapper.Tick` и тикает производство с учётом буста. `OfflineProgressService` рассчитывает оффлайн-доход с клампингом по `MaxOfflineSeconds`. `BoostService` хранит оставшееся время буста и сохраняет `BoostEndUtcTicks` в UTC, что позволяет корректно восстанавливать буст между сессиями. `AnalyticsService` — фасад над несколькими `IAnalyticsProvider` (сейчас реализован `ConsoleAnalyticsProvider`). `IIAPService` и `PurchaseCoordinator` абстрагируют Unity IAP, gameplay-код не зависит от API покупок.

## Конфигурация

Все параметры вынесены в ScriptableObject: `MachinesConfig` (список машин с Id, DisplayName, BaseProduction, ProductionPerLevel, UnlockCost, UpgradeCostBase, UpgradeCostMultiplier, UnlockedByDefault, MaxLevel), `BoostSettingsConfig` (Enabled, DurationSeconds, Multiplier), `OfflineSettingsConfig` (MaxOfflineSeconds). Формулы экономики: производство машины — `BaseProduction + ProductionPerLevel × Level`; стоимость апгрейда — `UpgradeCostBase × UpgradeCostMultiplier^Level`; boost — множитель на всё производство на указанное время; offline — `TotalProduction × min(DeltaSeconds, MaxOfflineSeconds)`.

## События аналитики

`game_started` — при инициализации игровой сцены. `machine_unlocked` — после успешного открытия машины, с параметрами `id`, `cost`, `balance`. `machine_upgraded` — после успешного апгрейда, с параметрами `id`, `level`, `balance`. `boost_started` и `boost_finished` — при активации и истечении буста. `offline_income_applied` — при возврате в игру, с параметрами `seconds`, `amount`. `purchase_succeeded` и `purchase_failed` — при завершении IAP-операции, с параметрами `product_id`, `result`.

## IAP

Реализован один consumable-продукт `coins_pack_small`, дающий 500 монет. В Editor работает Fake Store, покупки проходят без Google Play. В билде на Android Fake Store недоступен — нужен реальный продукт в Google Play Console. Абстракция `IIAPService` инкапсулирует Unity IAP v5, `PurchaseCoordinator` связывает успешные покупки с начислением валюты и отправкой аналитики.

## Тесты

В `Assets/_Project/Tests/EditMode/` находятся 23 EditMode теста: `EconomyServiceTests` (5), `MachineServiceTests` (9), `OfflineProgressServiceTests` (4), `BoostServiceTests` (5). Запускаются через `Window → General → Test Runner → EditMode → Run All`. Все тесты зелёные.

## Git

Разработка велась в Git с использованием Conventional Commits (`feat:`, `fix:`, `chore:`, `docs:`, `test:`). Основная ветка — `main`. `.gitignore` настроен под Unity: `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Builds/` игнорируются.

## Известные ограничения

Fake Store для IAP работает только в Editor — для Android нужен реальный продукт в Google Play Console. Firebase Remote Config не подключён, но интерфейс `IStaticDataService` готов к замене источника. UI без анимаций — базовые TextMeshPro и Button. При выходе из игры с активным Boost множитель применяется ко всему оффлайн-интервалу, если буст не истёк к моменту возврата — точное разделение интервала на «до конца буста» и «после» осталось незавершённым. PlayMode тесты не реализованы. Профилирование Android build не проводилось.

## Что можно улучшить

Подключить Firebase Remote Config с fallback на локальные SO через композитный `IGameConfig`. Добавить реальный Analytics Provider (AppMetrica или Firebase Analytics) поверх существующей архитектуры. Заменить `Resources` на Addressables для асинхронной загрузки. Добавить PlayMode тесты для `GameStateMachine` и `GameplayBootstrapper`. Добавить UI-анимации через DOTween или UniTask-based transitions. Исправить расчёт Boost + Offline в переходном случае через двухфазный расчёт. Провести профилирование Android build и оптимизировать аллокации в UI-обновлении.

## Время работы

Примерное фактическое время: **~5 часов**.

## Что осталось незавершённым

Firebase Remote Config (доп. задание). AppMetrica или Firebase Analytics (доп. задание). PlayMode тесты (доп. задание). Профилирование Android build (доп. задание). Корректная обработка пересечения Boost и Offline.

## Что сделали бы следующим шагом

Подключили бы Firebase Remote Config через композитный `IGameConfig` (Remote поверх Local). Добавили бы реальный Analytics Provider (AppMetrica). Провели бы профилирование на устройстве и оптимизировали бы частоту обновления UI (сейчас раз в 0.1s, можно перейти на событийную модель с троттлингом). Исправили бы расчёт Boost + Offline.
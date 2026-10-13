# &#x20;Mini Factory

# 

# Тестовое задание: мобильная idle-игра с машинами, апгрейдами, boost и IAP.

# 

# &#x20;Стек

# 

# \- Unity 6000.3.22f1

# \- Zenject (DI)

# \- UniTask (async)

# \- Unity IAP v5 (Fake Store)

# \- Newtonsoft.Json (save)

# \- NUnit (EditMode тесты)

# 

# &#x20;Запуск

# 

# 1\. Открыть проект в Unity Hub (Unity 6000.3.22f1).

# 2\. Открыть сцену `Assets/\_Project/Scenes/Bootstrap.unity`.

# 3\. Play.

# 

# &#x20;Android build

# 

# 1\. `File → Build Settings → Android → Switch Platform`.

# 2\. `Player Settings → Other Settings`:

# &#x20;  - `Scripting Backend = IL2CPP`

# &#x20;  - `Target Architectures = ARM64`

# 3\. `Build → Build and Run`.

# 

# &#x20;Архитектура

# 

# \- `GameStateMachine` — Bootstrap → LoadProgress → GameLoop.

# \- `GlobalInstaller` — сервисы, живущие всю жизнь приложения.

# \- `GameInstaller` — сервисы игровой сцены.

# \- `StaticDataService` — конфиги из `Resources/Configs`.

# \- `SaveLoadService` — PlayerPrefs + JSON.

# \- `PurchaseCoordinator` — покупки, независимые от Unity IAP.

# 

# &#x20;Известные ограничения

# 

# \- UI без анимаций.

# \- IAP работает только через Fake Store.

# \- Remote Config не подключён (заложен интерфейс).

# 

# &#x20;Время работы

# 

# \~5 часов.


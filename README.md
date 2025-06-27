# 🎮 Geometry Dash Key Blocker

<div align="center">

![Version](https://img.shields.io/badge/version-1.0-blue.svg?style=for-the-badge)
![Platform](https://img.shields.io/badge/platform-windows%2010%2F11-lightgrey.svg?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0+-512BD4?style=for-the-badge&logo=dotnet)
[![License](https://img.shields.io/badge/license-MIT-green.svg?style=for-the-badge)](LICENSE)


*Программа для блокировки клавиши стрелки вверх в Geometry Dash при записи хэндкама под макрос*

</div>

---

## 📝 Назначение

Данная программа предназначена для записи хэндкама (handcam) под макрос в Geometry Dash. Она блокирует клавишу стрелки вверх в игре, что позволяет записать видео с руками на клавиатуре, которое будет соответствовать макросу.

## 🚀 Как использовать

1. 👉 Запустите программу
2. 🎮 Запустите Geometry Dash
3. ⚙️ Программа автоматически найдет процесс игры
4. 🔄 Используйте кнопку "БЛОКИРОВКА ВКЛЮЧЕНА/ОТКЛЮЧЕНА" для управления блокировкой
5. ⚡ Для быстрого переключения используйте горячую клавишу (по умолчанию F6)
6. 🎯 Следите за статусом программы в верхней части окна

## ✨ Особенности

<table>
  <tr>
    <td>🔒</td>
    <td>Блокирует только клавишу стрелка вверх</td>
  </tr>
  <tr>
    <td>🎯</td>
    <td>Работает только когда окно Geometry Dash активно</td>
  </tr>
  <tr>
    <td>🔄</td>
    <td>Не влияет на работу клавиши в других программах</td>
  </tr>
  <tr>
    <td>⚡</td>
    <td>Горячие клавиши для быстрого управления</td>
  </tr>
  <tr>
    <td>⚙️</td>
    <td>Настраиваемые параметры через меню настроек</td>
  </tr>
  <tr>
    <td>📝</td>
    <td>Ведение логов для отладки</td>
  </tr>
  <tr>
    <td>🔔</td>
    <td>Уведомления в системном трее</td>
  </tr>
  <tr>
    <td>📌</td>
    <td>Опция "Поверх всех окон"</td>
  </tr>
</table>

## ⚙️ Настройки

- 🎮 Имя процесса игры (если отличается от стандартного)
- ⌨️ Горячая клавиша для включения/отключения блокировки
- 📝 Включение/отключение логирования
- 📌 Режим "Поверх всех окон"
- 🔔 Настройка уведомлений

## 💻 Системные требования

<table>
  <tr>
    <td>🖥️</td>
    <td><b>ОС:</b></td>
    <td>Windows 10/11</td>
  </tr>
  <tr>
    <td>📦</td>
    <td><b>Среда выполнения:</b></td>
    <td>.NET 8.0 или выше</td>
  </tr>
</table>

## ⚠️ Важно

> Программа предназначена исключительно для записи хэндкама под уже существующий макрос. Используйте её ответственно и только в целях создания контента.

## 🔄 Обновления 1.0 (27-06-25)

- Добавлен графический интерфейс
- Добавлены настройки программы
- Добавлена поддержка горячих клавиш
- Добавлено логирование
- Добавлены уведомления в трее
- Улучшена стабильность работы

## 📥 Установка

### Через релизы (рекомендуется)

1. 📦 Перейдите на страницу [Releases](https://github.com/DEN41R/GeometryDashKeyBlocker/releases)
2. ⬇️ Скачайте последний релиз `GeometryDashKeyBlocker.exe`
3. 🚀 Запустите скачанный файл

### Сборка из исходников

#### Требования для сборки

<table>
  <tr>
    <td>🛠️</td>
    <td><b>SDK:</b></td>
    <td>.NET 8.0 SDK</td>
  </tr>
  <tr>
    <td>👨‍💻</td>
    <td><b>IDE:</b></td>
    <td>Visual Studio 2022 или VS Code с C# расширением</td>
  </tr>
  <tr>
    <td>📦</td>
    <td><b>Система сборки:</b></td>
    <td>dotnet CLI</td>
  </tr>
</table>

#### Шаги сборки

1. 📂 Клонируйте репозиторий:
```bash
git clone https://github.com/denchir/GeometryDashKeyBlocker.git
cd GeometryDashKeyBlocker/src
```

2. 🔨 Соберите проект:
```bash
# Отладочная сборка
dotnet build

# Релизная сборка
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

3. 📁 Готовый файл будет находиться в:
   - Отладочная версия: `bin/Debug/net8.0-windows/win-x64/GeometryDashKeyBlocker.exe`
   - Релизная версия: `bin/Release/net8.0-windows/win-x64/publish/GeometryDashKeyBlocker.exe`

---

<div align="center">

### 🌟 Удачной записи хэндкама! 🌟

</div> 

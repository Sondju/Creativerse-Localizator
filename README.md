# Creativerse Universal Localizator

An open-source, flexible localization framework for **Creativerse** powered by MelonLoader. It allows modders and players to create, share, and organize custom translations for any language using a structured folder layout.

[Русское описание находится ниже](#russian-description)

---

## Features
* **Universal Language Support:** Easily translate the game into any language by editing clean JSON files.
* **Hierarchical Folder Structure:** Group your translations dynamically (e.g., `Furniture/Beds/Bed.json`). The mod recursively scans all folders.
* **Built-in Dumper:** Generate an up-to-date dump of all game strings directly from the main menu with a single keypress.
* **Context Preservation:** Every translation entry stores the original English string alongside its term key to help you stay oriented.
* **Optimized Performance:** Uses fast RAM dictionaries to inject text seamlessly into the game's I2.Loc framework without causing stutters.

## Installation
1. Download and install [MelonLoader](https://github.com/LavaGang/MelonLoader/releases).
2. Download the latest version of **Creativerse Localizator** and drop the `.dll` into your game's `Mods` folder.
3. Launch the game once to automatically generate the `Mods/Localization/` folder structure.

## How to Dump Strings
If you need a fresh list of game strings or want to see new items added in a game update:
1. Start the game and wait until you reach the **Main Menu**.
2. Press the **F10** key on your keyboard.
3. The mod will scan the game's databases and save a new `.json` file in Mods folder containing all extractable keys and English text. Use this file as a template for your translation.

## How to Create a Translation
Go to `Mods/Localization/` and create one or multiple `.json` files. You can split them into subfolders however you like. 

### JSON Structure Example
Every file must follow this structural layout:

```json
{
  "Entries": [
    {
      "Key": "item.bed",
      "Original": "Bed",
      "Translation": "Kровать"
    },
    {
      "Key": "block.bed",
      "Original": "Bed",
      "Translation": "Кровать"
    }
  ]
}
```

### Recommended Directory Layout
For large translation projects, we highly recommend separating game entities into logical subfolders for easier maintenance:
```text
Mods/
└── Localization/
    ├── UI/
    │   └── MainMenu.json
    └── Furniture/
        ├── Chairs/
        │   └── WoodenChair.json
        └── Beds/
            └── Bed.json
```

---

<a name="russian-description"></a>
# Универсальный локализатор Creativerse

Открытый и гибкий фреймворк для локализации игры **Creativerse** на базе MelonLoader. Он позволяет моддерам и игрокам создавать, распространять и структурировать кастомные переводы на любой язык с помощью удобного разделения по папкам.

## Особенности
* **Поддержка любых языков:** Легко переводите игру на любой язык, изменяя чистые JSON-файлы.
* **Иерархическая структура папок:** Группируйте ваши переводы так, как вам удобно (например, `Furniture/Beds/Bed.json`). Мод автоматически сканирует все поддиректории.
* **Встроенный дампер:** Создавайте актуальный дамп всех игровых строк прямо из главного меню нажатием одной кнопки.
* **Сохранение контекста:** Каждая запись хранит оригинальную английскую строку рядом с техническим ключом термина, что помогает ориентироваться при переводе.
* **Высокая оптимизация:** Использует быстрые кэш-словари в ОЗУ для бесшовной инъекции текста в движок I2.Loc без микрофризов в игре.

## Установка
1. Скачайте и установите [MelonLoader](https://github.com/LavaGang/MelonLoader/releases).
2. Скачайте актуальную версию **Creativerse Localizator** и поместите `.dll` файл в папку `Mods` в директории игры.
3. Запустите игру один раз, чтобы мод автоматически создал необходимую директорию `Mods/Localization/`.

## Как сделать дамп строк
Если вам нужен свежий список игровых текстов или вы хотите перевести новые предметы после обновления игры:
1. Запустите игру и дождитесь появления **Главного меню**.
2. Нажмите клавишу **F10** на клавиатуре.
3. Мод отсканирует базы данных игры и сохранит новый `.json` файл в папку Mods, содержащий все доступные ключи и оригинальный английский текст. Используйте его как шаблон для перевода.

## Как делать перевод
Перейдите в папку `Mods/Localization/` и создайте один или несколько `.json` файлов. Вы можете распределять их по подпапкам в любом удобном порядке.

### Пример структуры JSON-файла
Каждый файл перевода должен строго соответствовать следующему формату:

```json
{
  "Entries": [
    {
      "Key": "item.bed",
      "Original": "Bed",
      "Translation": "Кровать"
    },
    {
      "Key": "block.bed",
      "Original": "Bed",
      "Translation": "Кровать"
    }
  ]
}
```

### Рекомендуемая структура папок
Для крупных проектов локализации мы советуем разделять игровые объекты по логическим папкам, чтобы перевод было проще редактировать и дополнять:
```text
Mods/
└── Localization/
    ├── UI/
    │   └── MainMenu.json
    └── Furniture/
        ├── Chairs/
        │   └── WoodenChair.json
        └── Beds/
            └── Bed.json
```

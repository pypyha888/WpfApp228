# ApplicationData (EF Database First)

В проект добавлена структура под шаг 2.2/2.3:

- `ApplianceStoreEntities.edmx`
- `ApplianceStoreEntities.Context.tt`
- `ApplianceStoreEntities.tt`
- `ApplianceStoreEntitiesConnection.cs`

## Как подключена БД в коде

`SqlStoreRepository` использует строку подключения из `App.config` с именем:

`ApplianceStoreEntities`

Это соответствует требованию подключения через сохранённую строку в конфигурации.

## Дальше в Visual Studio

Откройте `ApplianceStoreEntities.edmx` в дизайнере и выполните Database First генерацию:

1. Update Model from Database / New ADO.NET Entity Data Model.
2. Выберите сервер `(localdb)\\MSSQLLocalDB`.
3. Выберите базу `ApplianceStoreISDb` (или вашу учебную БД).
4. Сохраните connection string как `ApplianceStoreEntities`.
5. После генерации Visual Studio создаст `.Designer`/`Context` классы автоматически.

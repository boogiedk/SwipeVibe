
## Проект

Демо-проект для курса "Highload Architect"


## Как запустить

**Запуск исходного кода локально:**
1. Скачать исходный код проекта через сайт `.zip` **либо** с помощью команды
```sh
git clone https://github.com/boogiedk/SwipeVibe.git
```
2. Установить SDK и Runtime [.Net Core 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
3. Перейти в директорию c исходным кодом:
```sh
cd SwipeVibe
```
4. Выполнить команду по сборке исходного кода:
```sh
dotnet build
```
5. Перейти в директорию со сборкой с помощью команды:
```sh
cd SwipeVibe.Backend\bin\Debug\net8.0\
```
6. Выполнить команду для запуска приложения:
```sh
dotnet SwipeVibe.Backend.dll
```
7. Скачать, установить [PostgreSQL](https://www.postgresql.org/download/), а затем настроить на порт `5432` **либо** запустить docker контейнер с помощью команды:
```sh
docker run --name swipe-vibe-db-container -p 5432:5432 -e POSTGRES_USER=swipe-vibe-app -e POSTGRES_PASSWORD=swipe-vibe-app -e POSTGRES_DB=swipe-vibe-app -d postgres:latest
```
8. Перейти по адресу в браузере: `http://localhost:5000`

**Запуск docker-compose с приложением (Linux):**
1. Скачать исходный код проекта через сайт `.zip` **либо** с помощью команды:
```sh
git clone https://github.com/boogiedk/SwipeVibe.git
```
2. Перейти в директорию c исходным кодом:
```sh
cd SwipeVibe
```
3. Выполнить команду для запуски docker-compose:
```sh
docker compose up
```
4. Перейти по адресу в браузере: `http://localhost:5000`


## Как работать

Для взаимодействия с приложением можно воспользоваться [коллекцией запросов для Postman](./Documentation/SwipeVibeCollection.postman_collection.json) **либо** вручную вызывать методы в UI Swagger приложения, который доступен по адресу [http://194.87.238.195:81](http://194.87.238.195:81).

**Порядок действия для работы со Swagger UI:**
1. Создаем пользователя через ```/api/v1/users```, сохраняя ```UserId```, который вернет метод;
2. Логиним пользователя в системе через ```/api/v1/users/login```, указывая номер телефона и пароль, указанный при создании пользователя. Токен, который вернул метод, добавляем в Authorize в UI Swagger (обычный Bearer Token в Authorization заголовка);
3. Возвращаем анкету пользователя через ```/api/v1/users/{userId}```
4. Получаем список пользователей, которые подходят по фильтру через ```/api/v1/users/search```;

**Обратить внимание**
1. Нельзя создать пользователя с одним и тем же Msisdn
2. Список пользователей не возвращает анкету пользователя, который делает запрос


## Используемые технологии

* [.NET Core](https://github.com/dotnet)
* [EF Core](https://github.com/dotnet/efcore)
* [PostgreSQL](https://www.postgresql.org)
* [Swagger](https://swagger.io/)
* [Github Action](https://github.com/features/actions)
* [Docker](https://www.docker.com/)

# Парсер сайта Aptekf.ru
***
Проект создает базу данных, затем получает с сайта apteka.ru актуальный список товаров и их цены, после чего сохраняет эту информацию в базу. Список товаров хранится в базе и структурирован по категориям и подкатегориям. Через WebAPI предоставляется возможность получать товары по названию категории, идентификатору категории или названию товара.

Программа отдельно парсит категории товаров раз в месяц и отдельно товары раз в неделю. При первом запуске парсятся сначала категории потом товары
***
## инструкция по настройке
***

* В файле __appsettings.json__ изменить строку подключения к БД __"DefaultConnection"__ на актуальное значение;

* В том же фалйе __appsettings.json__ подключить вписать актуальные данные прокси серверов

* При необходимости можно изменить график парсинга в папке Hangfire в файле __"HanfireMetods.cs"__

* Запустить программу "Program.cs"

### Автор
* Агаев Вели ShantaelVeli
* GitHub https://github.com/ShantaelVeli

***
## CURL запросы
***

### Запросы Search
#### 1
curl --location 'http://localhost:5178/api/Data/search' \
--header 'Content-Type: application/json' \
--header 'Accept: text/plain' \
--data '{
  "app": {
    "appId": "string",
    "appSecret": "string"
  },
  "searchPhraseList": [
    "лим"
  ],
  "waitTimeout": 0,
  "maxProductsCount": 559
}'

#### 2

curl --location 'http://localhost:5178/api/Data/search' \
--header 'Content-Type: application/json' \
--header 'Accept: text/plain' \
--data '{
  "app": {
    "appId": "string",
    "appSecret": "string"
  },
  "searchPhraseList": [
    "лимфомиозот"
  ],
  "waitTimeout": 0,
  "maxProductsCount": 559
}'
#### 3
curl --location 'http://localhost:5178/api/Data/search' \
--header 'Content-Type: application/json' \
--header 'Accept: text/plain' \
--data '{
  "app": {
    "appId": "string",
    "appSecret": "string"
  },
  "searchPhraseList": [
    "ЛИМфомиозОт"
  ],
  "waitTimeout": 0,
  "maxProductsCount": 559
}'

### Запросы Detail

#### 1
curl --location 'http://localhost:5178/api/Data/details' \
--header 'Content-Type: application/json' \
--header 'Accept: text/plain' \
--data '{
  "app": {
    "appId": "string",
    "appSecret": "string"
  },
  "canLoadAttachments": false,
  "productLinks": [
    "https://apteka.ru/product/limfomiozot-kapli-dlya-priema-vnutr-gomeopaticheskie-30-ml-5e3266d1f5a9ae0001407727/"
  ]
}'
#### 2
curl --location 'http://localhost:5178/api/Data/details' \
--header 'Content-Type: application/json' \
--header 'Accept: text/plain' \
--data '{
  "app": {
    "appId": "string",
    "appSecret": "string"
  },
  "canLoadAttachments": false,
  "productLinks": [
    "https://apteka.ru/product/limfomiozot-rastvor-dlya-vnutrimyshechnogo-vvedeniya-gomeopaticheskogo-primeneniya-11-ml-ampuly-5-sht-5e3266d1f5a9ae0001407728/"
  ]
}'
#### 3
curl --location 'http://localhost:5178/api/Data/details' \
--header 'Content-Type: application/json' \
--header 'Accept: text/plain' \
--data '{
  "app": {
    "appId": "string",
    "appSecret": "string"
  },
  "canLoadAttachments": false,
  "productLinks": [
    "https://apteka.ru/product/nabor-mastopol-60-tabl--neoklimsal-60-tabl-68da181d166f8ce2be21c0dc/"
  ]
}'
# CosmosPOC
CosmosDB POC to learn how to work with CosmosDB and determine the behaviour of the continuation token.

1. Does the token grow between requests?
2. Does the token store information about the page size?
3. Does the token store information about the query itself?


## Endpoints

### Get all items
`GET /api/Item`


### Get items WHERE name = "NewName"
This flag allows you to switch between the default SELECT all items query and a filtered query with a WHERE clause

`GET /api/Item?change=true`


### Get items paged by specifying page size (default is 50)
`GET /api/Item?limit=50`


### Get items specifying the continuation token
`GET /api/Item?continuation_token=`

### Post items
It's a bit counterintuitive but this request will create 200 Items in the DB

`POST /api/Item`

with body:

```json
{
    "id":"someId",
    "name":"someName"
}
```

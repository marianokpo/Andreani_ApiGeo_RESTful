# Andreani_ApiGeo_RESTful

Estas API desarrolladas en .NET Core 3.1 utilizando SQL Server.

## Estado

En Desarrollo

## Uso

GET http://*:5001/geocodificar?id=x

Donde x corresponde al numero de id que quiere consultar.


POST http://*:5001/geocodificar {calle: “”,numero: “”,ciudad: “”,código_postal: ””provincia: “”,pais: “}

Donde se envia un JSon a travez de POST para almacenar la informacion y solicitar la ubicacion a la otra api con OpenStreetMap.

## Que falta implementar

- Bróker de mensajería

- Segunda Api con OpenStreetMap

- Docker

## Contribuciones

Andreani

## License
[MIT](https://choosealicense.com/licenses/mit/)
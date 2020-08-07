# Andreani_ApiGeo_RESTful

Estas API desarrolladas en .NET Core 3.1 utilizando SQL Server, ademas utiliza OpenStrepMap y RabbitMQ.

## Estado

En Desarrollo

## Uso

GET http://*:5001/geocodificar?id=x

Donde x corresponde al numero de id que quiere consultar.


POST http://*:5001/geocodificar {calle: “”,numero: “”,ciudad: “”,código_postal: ””provincia: “”,pais: “}

Donde se envia un JSon a travez de POST para almacenar la informacion y solicitar la ubicacion a la otra api con OpenStreetMap.

- Bróker de mensajería RabbitMQ

- Api con OpenStreetMap

- Docker (2 container, una para cada App)

## Contribuciones

Andreani

## License
[MIT](https://choosealicense.com/licenses/mit/)
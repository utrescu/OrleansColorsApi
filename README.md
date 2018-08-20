# API de traduccions dels colors

La idea és desenvolupar una API en Net Core fent servir un sistema d'actors. En concret [Orleans](https://dotnet.github.io/orleans/index.html). Sembla que és semblant a Akka però més senzill.

Els avantatges dels sistemes d'actors és que simplifiquen la concurrència, són fàcilment escalables i etc... Millor que en mireu els avantatges a la seva web ;-)

He activat el Dashboard per defecte. S'hi accedeix amb http://localhost:8080:

![Dashboard](README/orleans-dashboard.png)

## Què he fet?

El que hi ha implementat és un sistema de traducció del codi RGB als diferents idiomes ... (ho sé, fa plorar).

La idea és que quan algú demani per un codi RGB li digui quin és el nom que té aquest color en diferents idiomes

    FF0000 -> català: vermell, castellà: rojo, anglès: red

El sistema persisteix automàticament les dades que se li entren en una base de dades MySQL de manera que sobreviu a les aturades del servei.

## Iniciar el programa

El programa necessita un sistema gestor de base de dades en marxa, que s'hi hagin creat les taules pertinents (que estan en scripts en la carpeta **OrleansAdoNetContent** del projecte **Silo**)

> La configuració de la base de dades està en un ConnectionString al fitxer **Program.cs** del projecte **Silo**. Algun dia ho milloraré :-P

Primer s'inicia el Silo:

    cd Silo
    dotnet run

I després l'API

    cd colorsAPI
    dotnet run

## Funcionament

Per ara només hi ha dos mètodes un per afegir una traducció i un per veureles (encara no es fan comprovacions)

Es poden veure les traduccions d'un color enviant un GET a /api/color/xxx (on xxx és el codi RGB en hexadecimal)

    http --verify=no  https://localhost:5001/api/colors/FFFF00

La resposta serà un document JSON amb les traduccions que hi hagi

    HTTP/1.1 200 OK
    Content-Type: application/json; charset=utf-8
    Date: Mon, 20 Aug 2018 17:58:19 GMT
    Server: Kestrel
    Transfer-Encoding: chunked

    {
        "id": "FFFF00",
        "names": [
            {
                "language": "catalan",
                "name": "groc"
            },
            {
                "language": "spanish",
                "name": "amarillo"
            },
            {
                "language": "english",
                "name": "yellow"
            },
            {
                "language": "french",
                "name": "jaune"
            }
        ]

Si el color no té cap traducció tornarà el document sense res a l'array 'names':

    HTTP/1.1 200 OK
    Content-Type: application/json; charset=utf-8
    Date: Mon, 20 Aug 2018 18:17:36 GMT
    Server: Kestrel
    Transfer-Encoding: chunked

    {
        "id": "000000",
        "names": []
    }

També dóna error si es passa alguna cosa que no sigui un codi RGB ...

    HTTP/1.1 400 Bad Request
    Content-Type: application/json; charset=utf-8
    Date: Mon, 20 Aug 2018 20:27:24 GMT
    Server: Kestrel
    Transfer-Encoding: chunked

    {
        "message": "Incorrect RGB Code"
    }

Es pot enviar una nova traducció al sistema enviant un POST amb l'Id del color com a paràmetre GET (per fer-ho diferent) i les dades de la traducció en el cos del missatge en format JSON

    http --verify=no  post https://localhost:5001/api/colors?id=FFFF00 Language="french" name="jaune"

Això ha enviat de contingut una cosa com aquesta:

```json
{
  "language": "french",
  "name": "jaune"
}
```

Si tot ha anat bé el sistema contesta amb un 200 Ok

    HTTP/1.1 200 OK
    Content-Type: application/json; charset=utf-8
    Date: Mon, 20 Aug 2018 20:22:33 GMT
    Server: Kestrel
    Transfer-Encoding: chunked

    {
        "message": "Translation added"
    }

També es pot modificar la traducció d'un idioma amb el mètode PUT (es canvia la traducció que coinicideixi amb el valor de _language_.

    http --verify=no  put https://localhost:5001/api/colors/FFFF00 Language="french" name="jaune"

Espectacular oi?

No gaire, però l'avantatge del sitema és que es pot escalar molt més fàcilment que un sistema tradicional creant clusters de Silos, etc... (llegiu la documentació)

## API

Les peticions implementades són:

| URL                    | Mètode                            |
| ---------------------- | --------------------------------- |
| GET /api/color/ff0000  | Obtenir les traduccions de FF0000 |
| POST /api/color/ff0000 | Afegir una traducció de FF0000    |
| PUT /api/color/ff0000  | Modificar una traducció de FF0000 |

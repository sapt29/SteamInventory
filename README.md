
# Steam Inventory

Backend and frontend project used to show the information about a steam profile and it's inventory.

## Requirements

[MongoDB](https://www.mongodb.com/try/download/atlascli)

[.NET Core 8](https://dotnet.microsoft.com/es-es/download/dotnet/8.0)

[Node & Npm](https://docs.npmjs.com/downloading-and-installing-node-js-and-npm)

[Visual studio Code (Optional)](https://visualstudio.microsoft.com/es/downloads/)

[Visual studio 2022 (Optional)](https://visualstudio.microsoft.com/es/downloads/)

## Set up project

### Backend project

In the appsettings.json file, use your ApiKey generated from the https://www.steamwebapi.com/ site

For the database setup, go to the appsettings.json file, and check if the MongoDbConfiguration parameters meet the requirements that you already setup or if you need to change something or configure the database as it is on the file.

Please take into account that previously you have intalled and configured MongoDb

Once checked all that, you only have to right click on the SteamInventory.Api project and select the option Set as startup project. Finally you can click the run button with the TestApi configuraion selected. If everything goes as expected, the service will be running locally at https://localhost:7155. You can change the port or the address on the launchSettings.json file.

### Frontend project

Open a terminal an follow the steps:


```bash
  npm install
```

Start the server

```bash
  npm run dev
```
Once you do that, the console will show the localhost direction where the frontend is running, usually is http://localhost:5173/. You copy the direction on a browser and then you can see it running.

Just in case you have changed the backend running direction or port you have to update it at the app.tsx file.

Following those steps you should start and run the project locally.

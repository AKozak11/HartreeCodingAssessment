# ***HARTREE CODING ASSESSMENT***<br/>
<pre>
<b>System requirements ( test environment ):</b>
Operating Sytem: Microsoft Windows 10 Pro ( version 10.0.19045 )
Target Framework: .NET core - net6.0
Docker: Docker version 20.10.22, Docker Compose version v2.15.1
</pre>
<pre>
<b>Step 1.</b> Run build.ps1 from powershell console<br/>
.\build.ps1<br/>
This will:
    * build images and run containers for kafka zookeeper, kafka broker, and mssql
    * create a new kafka topic called 'RANDOM_NUMBER_DATA' that the services will consume and produce messages to
    * create a database in the mssql container named 'RandomNumberData' with one table named 'dbo.Data' using the pre-generated migrations in the EntityConnector project
    * clean solution, restore nuget packages, build solution<br/>
<b>Step 2.</b> Run services<br/>
From .\RandomNumberConsumer\RandomNumberConsumer\ directory in a powershell console, run project<br/>
dotnet run<br/>
From .\RandomNumberProducer\RandomNumberProducer\ directory in a powershell console, run project<br/>
dotnet run<br/>
</pre>

##### Testing:<br/>
<pre>
Connect to the mssql server through SSMS (or command line tool) using the password in the docker-compose.yml file<br/>
Server Name: 127.0.0.1,1433
Auth: SQL Server Authentication
User: sa
Pass: Sup3rStr0ngP@SSwordBr0!<!--Not a super strong password 😁--><br/>
Go to .\RandomNumberConsumer\RandomNumberConsumer\bin\Debug\net6.0-windows\ directory in file explorer and double click 'RandommNumberConsumer-AddIn64.xll' file
There are two keys to watch for data [Key1, Key2]
In one cell type     =GetData("Key1")
In another cell type =GetData("Key2")<br/>
You will see the value in each cell changing as data is read from kafka
In RandomNumberConsumer powershell console you will see the service logging messages from kafka as well<br/>
To confirm excel has the latest data, stop the RandomNumberProducer service and note the values in excel
In SSMS look for latest records by time and compare values for most recent records for Key1 and Key2<br/>
select top(5) * from RandomNumberData.dbo.Data order by Time desc<br/>
Close excel
Re-open .\RandomNumberConsumer\RandomNumberConsumer\bin\Debug\net6.0-windows\RandommNumberConsumer-AddIn64.xll
Enter the keys to watch again. example: =GetData("Key1")
Latest value will be displayed</pre>
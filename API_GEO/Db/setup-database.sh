# Wait to be sure that SQL Server came up
sleep 90s

# Run the setup script to create the DB and the schema in the DB
# Note: make sure that your password matches what is in the Dockerfile
echo 'Please wait while sql server 2017 is running';

sleep 90s

echo 'Initializing db after 90 second';

/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P APIGEO2020# -d master -i create-database.sql
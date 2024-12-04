# Migrations

## Add migration

```
dotnet tool install --global dotnet-ef
export PATH="$PATH:/root/.dotnet/tools"
dotnet ef migrations add MIGRATION_NAME
```

## Apply migration

```
dotnet ef database update
```

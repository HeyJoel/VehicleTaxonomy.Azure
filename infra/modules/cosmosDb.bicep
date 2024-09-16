param location string
param project string
param uniqueSuffix string

@description('The name for the CosmosDb database')
param databaseName string = 'vehicle-taxonomy'

@description('The name for the CosmosDb container')
param containerName string = 'vehicle-taxonomy'

@description('The application deployment environment. Currently only "dev" and "prod" environment is supported.')
@allowed(['dev', 'prod'])
param environmentType string

param tags { *: string }

var configMap = {
  dev: {
    databaseThroughput: 1000
    useFreeTier: true
  }
  prod: {
    databaseThroughput: 1000
    useFreeTier: false
  }
}
var config = configMap[environmentType]

resource cosmosDbAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' = {
  name: toLower(take('cosmos-${project}-${environmentType}-${uniqueSuffix}', 44))
  location: location
  kind: 'GlobalDocumentDB'
  tags: tags
  properties: {
    enableFreeTier: config.useFreeTier
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
      }
    ]
  }

  resource database 'sqlDatabases' = {
    name: databaseName
    properties: {
      resource: {
        id: databaseName
      }
      options: {
        throughput: config.databaseThroughput
      }
    }

    // Mirror of: VehicleTaxonomyContainerDefinition.cs
    resource container 'containers' = {
      name: containerName
      properties: {
        resource: {
          id: containerName
          partitionKey: {
            paths: [
              '/parentPath'
            ]
            kind: 'Hash'
          }
          indexingPolicy: {
            indexingMode: 'consistent'
            includedPaths: [
              {
                path: '/parentPath/?'
              }
              {
                path: '/name/?'
              }
            ]
            excludedPaths: [
              {
                path: '/*'
              }
            ]
          }
        }
      }
    }
  }
}

output cosmosDbAccountName string = cosmosDbAccount.name
output cosmosDbDatabaseName string = databaseName

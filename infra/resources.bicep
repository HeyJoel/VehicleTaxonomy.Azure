param location string
param project string
param uniqueSuffix string
@description('Currently only "dev" environment is supported.')
@allowed(['dev'])
param environmentType string
param tags { *: string }

module logAnalytics 'modules/logAnalytics.bicep' = {
  name: '${deployment().name}-logAnalytics'
  params: {
    location: location
    project: project
    environmentType: environmentType
    tags: tags
  }
}

module importFileStorage 'modules/importFileStorage.bicep' = {
  name: '${deployment().name}-import-file-storage'
  params: {
    location: location
    project: project
    environmentType: environmentType
    tags: tags
    uniqueSuffix: uniqueSuffix
  }
}

module cosmosDb 'modules/cosmosDb.bicep' = {
  name: '${deployment().name}-cosmosDb'
  params: {
    location: location
    project: project
    environmentType: environmentType
    tags: tags
    uniqueSuffix: uniqueSuffix
  }
}

module functions 'modules/functionsApp.bicep' = {
  name: '${deployment().name}-functions-app'
  params: {
    location: location
    project: project
    environmentType: environmentType
    tags: tags
    uniqueSuffix: uniqueSuffix
    cosmosDbAccountName: cosmosDb.outputs.cosmosDbAccountName
    cosmosDbDatabaseName: cosmosDb.outputs.cosmosDbDatabaseName 
    importFileStorageAccountName: importFileStorage.outputs.storageAccountName
    logAnalyticsWorkspaceId: logAnalytics.outputs.logAnalyticsWorkspaceId
  }
}

output functionAppName string = functions.outputs.functionAppName
output functionBaseUrl string = functions.outputs.functionBaseUrl

param location string
param project string

@description('Currently only "dev" environment is supported.')
@allowed(['dev'])
param environmentType string

param tags { *: string }

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: toLower('laws-${project}-${environmentType}')
  location: location
  tags: tags
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
    workspaceCapping: {
      dailyQuotaGb: 1
    }
  }
}

output logAnalyticsWorkspaceId string = logAnalyticsWorkspace.id

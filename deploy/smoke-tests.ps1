param(
    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [string] $HostName
)

Describe 'Function app' {

    It 'Serves pages over HTTPS' {
        $request = [System.Net.WebRequest]::Create("https://$HostName/")
        $request.AllowAutoRedirect = $false
        $request.GetResponse().StatusCode |
            Should -Be 200 -Because "the website requires HTTPS"
    }

    It 'Does not serves pages over HTTP' {
        $request = [System.Net.WebRequest]::Create("http://$HostName/")
        $request.AllowAutoRedirect = $false
        $request.GetResponse().StatusCode | 
            Should -BeGreaterOrEqual 300 -Because "HTTP is not secure"
    }

    It 'Returns a success code from the make list endpoint' {
      $response = Invoke-WebRequest -Uri "https://$HostName/api/makes/" -SkipHttpErrorCheck
      Write-Host $response.Content
      $response.StatusCode |
            Should -Be 200 -Because "the makes endpoint is healthy"
    }

}
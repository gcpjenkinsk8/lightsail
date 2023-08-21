# Define your personal access token
$token = "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"

# Define repository details
$owner = "gcpjenkinsk8"
$repo = "lightsail"
$path = "builds"

# Construct the API URL
$apiUrl = "https://api.github.com/repos/$owner/$repo/contents/$path"

# Set up headers with the personal access token
$headers = @{
    "Authorization" = "Bearer $token"
    "User-Agent" = "PowerShell"
}

# Make the API request
$response = Invoke-RestMethod -Uri $apiUrl -Headers $headers

# Get the download URL from the response
$downloadUrl = $response.download_url

# Use Invoke-WebRequest to download the file
Invoke-WebRequest -Uri $downloadUrl -OutFile "build1.zip"

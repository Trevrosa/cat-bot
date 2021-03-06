# Author: Sankarsan Kampa (a.k.a. k3rn31p4nic)
# License: MIT

$STATUS=$args[0]
$WEBHOOK_URL=$args[1]

if (!$WEBHOOK_URL) {
  Write-Output "WARNING!!"
  Write-Output "You need to pass the WEBHOOK_URL environment variable as the second argument to this script."
  Write-Output "For details & guide, visit: https://github.com/DiscordHooks/appveyor-discord-webhook"
  Exit
}

Switch ($STATUS) {
  "success" {
    $EMBED_COLOR=3066993
    $STATUS_MESSAGE="Passed"
    Break
  }
  "failure" {
    $EMBED_COLOR=15158332
    $STATUS_MESSAGE="Failed"
    Break
  }
  default {
    Break
  }
}

$AVATAR="https://upload.wikimedia.org/wikipedia/commons/thumb/b/bc/Appveyor_logo.svg/256px-Appveyor_logo.svg.png"

if (!$env:APPVEYOR_REPO_COMMIT) {
  $env:APPVEYOR_REPO_COMMIT="$(git log -1 --pretty="%H")"
}

$AUTHOR_NAME="$(git log --pretty=format:'%aN' -n 1 "$env:APPVEYOR_REPO_COMMIT")"
$COMMITTER_NAME="$(git log --pretty=format:'%cN' -n 1 "$env:APPVEYOR_REPO_COMMIT")"
$COMMIT_SUBJECT="$(git log --pretty=format:'%s' -n 1 "$env:APPVEYOR_REPO_COMMIT")" -replace "`"", "'"
$COMMIT_MESSAGE=(git log --pretty=format:'%b' -n 1 "$env:APPVEYOR_REPO_COMMIT") -replace "`"", "'" | Out-String | ConvertTo-Json

if ($AUTHOR_NAME -eq $COMMITTER_NAME) {
  $CREDITS="`n$AUTHOR_NAME authored & committed" | ConvertTo-Json

}
else {
  $CREDITS="`n$AUTHOR_NAME authored & $COMMITTER_NAME committed" | ConvertTo-Json

}

# Remove Starting and Ending double quotes by ConvertTo-Json
$COMMIT_MESSAGE = $COMMIT_MESSAGE.Substring(1, $COMMIT_MESSAGE.Length-2)
$CREDITS = $CREDITS.Substring(1, $CREDITS.Length-2)

$BUILD_VERSION = [uri]::EscapeDataString($env:APPVEYOR_BUILD_VERSION)
$TIMESTAMP="$(Get-Date -format s)Z"
$WEBHOOK_DATA="{
  ""username"": ""AppVeyor"",
  ""avatar_url"": ""$AVATAR"",
  ""embeds"": [ {
    ""color"": $EMBED_COLOR,
    ""title"": ""Build #$env:APPVEYOR_BUILD_NUMBER $STATUS_MESSAGE"",
    ""url"": ""https://ci.appveyor.com/project/Trevrosa/cat-bot"",
    ""description"": ""**$COMMIT_SUBJECT** $CREDITS"",
    ""fields"": [
      {
        ""name"": ""Github"",
        ""value"": ""[``Main Page``](https://github.com/Trevrosa/cat-bot)"",
        ""inline"": false
      },
      {
        ""name"": ""Commit"",
        ""value"": ""[``$($env:APPVEYOR_REPO_COMMIT.substring(0, 7))``](https://github.com/$env:APPVEYOR_REPO_NAME/commit/$env:APPVEYOR_REPO_COMMIT)"",
        ""inline"": false
      },
      {
        ""name"": ""Version"",
        ""value"": ""``$env:APPVEYOR_BUILD_VERSION``"",
        ""inline"": false
      },
      {
        ""name"": ""Branch"",
        ""value"": ""``$env:APPVEYOR_REPO_BRANCH``"",
        ""inline"": false
      }
    ],
    ""timestamp"": ""$TIMESTAMP""
  } ]
}"

Invoke-RestMethod -Uri "$WEBHOOK_URL" -Method "POST" -UserAgent "AppVeyor-Webhook" `
  -ContentType "application/json" -Header @{"X-Author"="k3rn31p4nic#8383"} `
  -Body $WEBHOOK_DATA

Write-Output "[Webhook]: Successfully sent the webhook."

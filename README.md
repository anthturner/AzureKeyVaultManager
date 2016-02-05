![Build Status](https://dpeted.visualstudio.com/DefaultCollection/_apis/public/build/definitions/f172cbc4-c5dd-4e53-a795-ed5dc807800c/128/badge)

# Azure Key Vault Manager
Manage your Azure Key Vault with ease via a simple WPF application. This can help developers and deployment managers update keys and secrets, manage access policies, and roll over keys with just a few clicks.

## Configuration
The provided App.config provides four configuration fields, but requires only the first two:

```
<add key="ActiveDirectoryTenantId" value="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" />
```
This value is the Tenant ID for the Azure subscription containing the Key Vault(s) to manage. It can be found in the URL when you hover over the directory name in the Azure Management portal.


```
<add key="Subscription" value="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" />
```
This value is the Subscription ID for the Azure subscription containing the Key Vault(s) to manage. It can be found in the Subscriptions pane in the new Azure portal, or Settings -> Subscriptions in the old Azure portal.


```
<add key="KeyVaultClientId" value="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" />
```
This value is the Client ID for an application that will be used to access key/secret data from Key Vault. Effectively, this forces the non-management parts of the application to authenticate with an application service principal rather than the default user principal. This is optional; commenting it will simply force the application to authenticate with a user principal.


```
<add key="KeyVaultClientSecret" value="xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" />
```
This value is the Client Secret for an application that will be used to access key/secret data from Key Vault. This is optional; commenting it will simply force the application to authenticate with a user principal.

## Usage
The tree on the left side contains, in order, Vaults -> Keys/Secrets -> Versions.

**Vaults**: Clicking these will display a window to configure access policies for the given vault.

**Keys/Secrets**: Clicking these will select the active version of the selected key or secret and display a window to display and configure the selected key or secret version.

**Keys/Secret Versions**: Clicking these will select a given version of a key or secret and display a window to display and configure the selected key or secret version.

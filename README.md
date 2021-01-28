# bBotLibrary
Discord.Net.Bot - A small library used to make a base bot in seconds.

This package was created to speed up creation of bots for myself. Released to help anyone else who would benefit. Uses NuGet package Discord.Net but has no affiliation with Discord.Net. If outdated, feel free to contribute and create a pull request to update to latest Discord.Net version.

### Example project
An example project that uses the Discord.Net.Bot package is Lori's Angel. Lori's Angel is a bot that adds fun commands and games such as connect 4 with rendered game boards. You can invite Lori's Angel to your Discord Guild here: https://discordapp.com/oauth2/authorize?client_id=729696788097007717&scope=bot&permissions=44032
View the source code of Lori's Angel: https://github.com/byBlurr/lorisangel

Another example project that is just a basic bot will be added to the solution soon.

### Bigger Updates
#### Changes that you will need to change if you have updated recently
Check Lori's Angel to see how to make the changes...
- New BotCommand changes mean you will need to update the CommandHandler.RegisterCommands() override.
- New util classes instead of one big class, you will need to go through all occurances of the Util class and change them to the correct Util class. No method names were changed, the Util classes are as follows Util, EmojiUtil, StringUtil, ChannelUtil and MessageUtil

# TicketBot
keeps track of support tickets made in Discord.
<br>
requires dotnet runtime & SDK installed to run.
<br>
use "dotnet run" from inside the project folder to run the bot.
# Token Commands
when the bot asks for a token, you can also enter some commands instead and it will tell the difference between them and an actual token.
<br>
List of Token Commands:
<br>
notYet - exits the program, this is used for debugging
<br>
read - read the token in the currently existing token file (will fuck up if there is no token file)
<br>
write - write a new token to the file (kinda broken at the moment, i suggest creating the token file manually)
<br>
Creating a token file manually: make a text file called "token.txt" in the project folder and put only the token in it, no whitespace, tabs, returns, newlines, feeds, nothing, just the token.
# Bot Commands
while the bot is runing, any message sent in a discord channel it can see that starts with "!ticket " will be interpretted as a command by the bot. if whatever is typed after that doesn't match with anything in the list of commands, (including any extra whitespaces,) the command will return with a message stating "invalid command" and do nothing.
<br>
List of Discord Commands:
<br>
ping - returns "pong" (normally used in debugging, but it's cute so i'll keep it in release)
<br>

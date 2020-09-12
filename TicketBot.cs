//TicketBot by MetalLabs
using System;
//these came with the initialization
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//useful thingsies
using System.IO;
using System.Threading;
using System.Reflection;
using System.Data;
//discord stuffs ^w^
using Discord;
using Discord.Commands;
using Discord.WebSocket;
//xml reader
using System.ServiceModel.Syndication;

namespace TicketBot
{
    //make this public so it can be called from outside
    public class Program
    {
        //creates a new instance of DiscordSocketClient
        private DiscordSocketClient _client = new DiscordSocketClient();
        //variable to be assigned a token at runtime
        public static string botToken = ("notYet");
        //main program starting method is also public
        public static void Main(string[] args)
        {
            //startup variable
            bool answerIsNotValid = true;
            //while the user has not given an answer yet
            while (answerIsNotValid)
            {
                //ask permision to startup
                Console.WriteLine(@"Start TicketBot?");
                Console.WriteLine("");
                bool startupTime = answerHandlerYesOrNo();
                //check if the user said to close the program
                if (startupTime == false)
                {
                    //close the program
                    Environment.Exit(499);
                }
                answerIsNotValid = false;
            }
            Console.WriteLine("starting TicketBot...");
            Console.WriteLine("");
            //check if there is not a ticket file already
            if (!(File.Exists("tickets.txt")))
            {
                Console.WriteLine("Creating tickets file...");
                //if not, make it
                File.Create("tickets.txt");
                Console.WriteLine("Tickets file created.");
                Console.WriteLine("");
            }
            //ask user for a token
            Console.WriteLine("TicketBot needs a token:");
            //puts the token in the holder
            botToken = Console.ReadLine();
            Console.Clear();
            //checks if the user wants to end here
            if (botToken == ("notYet"))
            {
                //exits the program
                Environment.Exit(498);
            }
            //checks if the user wants to read the token from a file
            if (botToken == ("read"))
            {
                //reads the token from the token file
                botToken = File.ReadAllText("token.txt");
            }
            //checks if the user wants to write to the token file
            if (botToken == ("write"))
            {
                Console.WriteLine("");
                //checks if the file already exists
                if (File.Exists("token.txt"))
                {
                    //asks the user to overwrite the file
                    Console.WriteLine("A token file already exists, overwrite?");
                    Console.WriteLine("");
                    bool doOverWriting = answerHandlerYesOrNo();
                    //if user says no
                    if (doOverWriting == false)
                    {
                        //exit the program
                        Console.WriteLine("exiting...");
                        Environment.Exit(497);
                    }
                    //asks the user for a token to write to the file
                    Console.WriteLine("");
                    Console.WriteLine("Enter a token to write to the file:");
                    Console.WriteLine("");
                    string tokenWrite = Console.ReadLine();
                    //write token into file (still untested)
                    File.WriteAllText("token.txt", tokenWrite);
                }
                else
                {
                    //creates a token file
                    File.Create("token.txt");
                    //asks the user for a token to write to the file
                    Console.WriteLine("");
                    Console.WriteLine("Enter a token to write to the file:");
                    Console.WriteLine("");
                    string tokenWrite = Console.ReadLine();
                    //write token into file  (still untested)
                    File.WriteAllText("token.txt", tokenWrite);
                }
            }
            //checks if the token is null
            if ((botToken == (null)) || (botToken == ("")))
            {
                //exits the program
                Environment.Exit(496);
            }
            Console.WriteLine("");
            Console.WriteLine("Logging in TicketBot...");
            Console.WriteLine("");
            //sets up to catch exceptions
            try
            {
                //run the async threading method which starts the bot
                new Program().MainAsync().GetAwaiter().GetResult();
            }
            //if an exception occurs
            catch (Exception errorOutput)
            {
                //let the user know
                Console.WriteLine("");
                Console.WriteLine("An error occured: ");
                Console.WriteLine("");
                Console.WriteLine(errorOutput);
                Console.WriteLine("");
                Console.WriteLine("End of Line.");
                Console.WriteLine("");
                Console.WriteLine("Quitting Program...");
                Console.WriteLine("Press any key to close.");
                Console.ReadKey();
                Environment.Exit(495);
            }
        }
        //async threading method
        public async Task MainAsync()
        {
            //hooks log event to log handler method
            _client.Log += Log;
            //logs the bot in to discord
            await _client.LoginAsync(TokenType.Bot, botToken);
            //start connection-reconnection logic
            await _client.StartAsync();
            //activates message receiver when a message is received
            _client.MessageReceived += MessageReceived;
            //block the async main method from returning until after the application is exited
            await Task.Delay(-1);
        }
        //message receiver activates when a message is recieved
        private async Task MessageReceived(SocketMessage message)
        {
            //activates if the bot is pinged
            String mescon = message.Content;
            //makes the content a string
            mescon = mescon.ToString();
            //checks if it is a command 
            if (((mescon.StartsWith("!ticket")) == true) && (!(message.Author.IsBot)))
            {
                //lowers the case of the input
                mescon = mescon.ToLower();
                //command handlers go here
                if (mescon == ("!ticket ping"))
                {
                    await message.Channel.SendMessageAsync("pong");
                }
                //lists all current tickets
                else if ((mescon == ("!ticket all")) || (mescon == ("!ticket list")))
                {
                    string messageToSend = ("listing all tickets:");
                    await message.Channel.SendMessageAsync(messageToSend);
                    //grab all tickets
                    string ticketsList = File.ReadAllText("tickets.txt");
                    //check if the file is blank
                    if (string.IsNullOrEmpty(ticketsList))
                    {
                        ticketsList = ("no tickets");
                    }
                    //send results
                    await message.Channel.SendMessageAsync(ticketsList);
                }
                //creates a ticket
                else if ((mescon.StartsWith("!ticket create ")) || (mescon.StartsWith("!ticket open ")))
                {
                    //ignore leading length
                    int openOrCreate = 15;
                    //find out if open or create was used
                    if (mescon.StartsWith("!ticket open "))
                    {
                        //if open was used make leading length larger
                        openOrCreate = 13;
                    }
                    //grab subject line for ticket
                    string ticksubj = mescon;
                    ticksubj = ticksubj.Remove(0, openOrCreate);
                    //check the number of tickets (lines) in the file
                    int newTicketNumber = File.ReadAllLines("tickets.txt").Length;
                    //assign the next number in  line to the new ticket
                    newTicketNumber = newTicketNumber + 1;
                    //create the leading string
                    string leadingZeroes = leadingZeroFinder(newTicketNumber);
                    //open stream to the tickets file and append to the end of it
                    StreamWriter ticketStream = new StreamWriter("tickets.txt", true);
                    //find the user that sent the message
                    var userObject = message.Author;
                    //grab just their name, nothing else
                    string userName = userObject.ToString();
                    //add the ticket to the file
                    ticketStream.WriteLine(leadingZeroes + newTicketNumber + ": " + "open - " + ticksubj + "  --  " + userName);
                    //close the stream (always do this)
                    ticketStream.Close();
                    //show user that the ticket has been created
                    string confirmMsg = ("ticket created: " + newTicketNumber + ": " + ticksubj);
                    await message.Channel.SendMessageAsync(confirmMsg);
                }
                //closes a ticket
                else if (mescon.StartsWith("!ticket close "))
                {
                    //ignore the leading length
                    string whichToClose = mescon;
                    whichToClose = whichToClose.Remove(0, 14);
                    //turn the string into an integer
                    int numberToClose = (0);
                    bool result = int.TryParse(whichToClose, out numberToClose);
                    //if that fails...
                    if (!result)
                    {
                        //tell the user they fucked up
                        await message.Channel.SendMessageAsync("tickets can only be closed by their ticket integer");
                    }
                    //otherwise...
                    else if (result)
                    {
                        //read all file content into a variable
                        string fullList = File.ReadAllText("tickets.txt");
                        //get the leading zeroes value
                        string leadingZeroes = leadingZeroFinder(numberToClose);
                        /*
                        TODO: still need to implement the rest of this,
                        need to find the line in question and replace it
                        without touching the other lines
                        */
                    }
                }
                //if no matching command is found
                else
                {
                    //let the user know they fucked up
                    await message.Channel.SendMessageAsync("invalid  or malformed command");
                }
            }
        }
        //log handler method
        private Task Log(LogMessage logmsg)
        {
            //writes log to the console
            Console.WriteLine(logmsg.ToString());
            //tells caller that the task was completed
            return Task.FromResult(1);
        }
        //console yes or no handler
        public static bool answerHandlerYesOrNo()
        {
            //returns true if yes, false if no
            //endlessly loops until an answer is given
            while (true)
            {
                //ask the user for input
                Console.WriteLine("Y/N:");
                Console.WriteLine("");
                //take in the answer and assign it into a single-character variable
                ConsoleKeyInfo userEntry = Console.ReadKey();
                Console.WriteLine("");
                //check to see if the answer is a no
                if ((userEntry.KeyChar == 'n') || (userEntry.KeyChar == 'N'))
                {
                    return false;
                }
                //check to see if the answer is a yes
                if ((userEntry.KeyChar == 'y') || (userEntry.KeyChar == 'Y'))
                {
                    return true;
                }
                //if neither is chosen
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine(@"Invalid option, please press 'Y' or 'N'.");
                    Console.WriteLine("");
                }
            }
        }
        //finds out how many leading zeroes to add, takes an int and returns a string
        public static string leadingZeroFinder(int nTN)
        {
            //assign leading zeros variable to nothing
            string leaderN = ("");
            //if new line number doesn't have three digits
            if (nTN < 100)
            {
                //give it a leading zero
                leaderN = ("0");
            }
            //if it has fewer than two digits
            if (nTN < 10)
            {
                //give it two leading zeroes
                leaderN = ("00");
            }
            //return the leading zeroes
            return leaderN;
        }
    }
}


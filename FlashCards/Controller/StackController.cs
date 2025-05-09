using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using FlashCards.Data;
using FlashCards.Models;

namespace FlashCards.Controller
{
    public class StackController
    {

        public void InsertToStack(string stackName)
        {

            using (var connection = new SqlConnection(MockDatabase.GetConnectionString()))
            {
                connection.Open();

                // Insert new Stack
                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = @"INSERT INTO Stack (Name) VALUES (@stackName);";
                selectCmd.Parameters.AddWithValue("@stackName", stackName);

                // Runs the insert command
                selectCmd.ExecuteNonQuery();

                Console.WriteLine("Successfully created new stack");

                connection.Close();
            }
        }

        public void DeleteFromStack()
        {
            DisplayAllStacks();
            Console.WriteLine("---------------------------");
            Console.WriteLine("Choose a stack id to delete: ");
            Console.WriteLine("---------------------------");
            var stackId = Console.ReadLine();
            int cardId;
            if (!Int32.TryParse(stackId, out cardId))
            {
                Console.WriteLine("\nPlease enter a number\n");
                DeleteFromStack();
            }
            else
            {

                using (var connection = new SqlConnection(MockDatabase.GetConnectionString())) {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"DELETE from Stack WHERE Id = @stackId";
                tableCmd.Parameters.AddWithValue("@stackId", cardId);

                int rowCount = tableCmd.ExecuteNonQuery();
                if (rowCount == 0)
                {
                    System.Console.WriteLine($"\n\nRecord with Id {stackId} doesn't exist. \n\n");
                    DeleteFromStack();
                }

                else
                {
                    Console.WriteLine("\n Stack was successfully deleted");
                    Console.WriteLine("\n Press any key to return to main menu\n");
                }


                }

            }



        }

        public void UpdateStack()
        {
            DisplayAllStacks();
            Console.WriteLine("---------------------------");
            Console.WriteLine("Choose a stack id to edit: ");
            Console.WriteLine("---------------------------");
            var id = Console.ReadLine();

            int stackId;
            if (!Int32.TryParse(id, out stackId))
            {
                Console.WriteLine("\nPlease enter a number\n");
                UpdateStack();
            }

            else
            {

                Console.WriteLine("---------------------------");
                Console.WriteLine("Add new name of stack: ");
                Console.WriteLine("---------------------------");
                var newStackName = Console.ReadLine();

                using (var connection = new SqlConnection(MockDatabase.GetConnectionString()))
                {

                    connection.Open();

                    var selectCmd = connection.CreateCommand();
                    // Insert updated code item properties to database
                    selectCmd.CommandText = @"UPDATE Stack SET Name = @name WHERE Id = @id";
                    selectCmd.Parameters.AddWithValue("@name", newStackName);
                    selectCmd.Parameters.AddWithValue("@id", stackId);


                    // Check if id is in stack
                    int rowCount = selectCmd.ExecuteNonQuery();
                    if (rowCount == 0)
                    {
                        Console.WriteLine($"\n\nRecord with Id {stackId} doesn't exist. \n\n");
                        UpdateStack();
                    }

                    else {
                        Console.WriteLine("\nUpdated Stack Successfully\n");
                    }

                    connection.Close();

                }
            }
        }



        public int GetStackId(string stackName)
        {
            using (var connection = new SqlConnection(MockDatabase.GetConnectionString()))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT Id FROM Stack WHERE Name = @StackName";

                selectCmd.Parameters.AddWithValue("@StackName", stackName);

                using (var reader = selectCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }

                    else {
                        Console.WriteLine("Could not find a Stack with that name");
                        return -1;
                    }
                }

                connection.Close();
            }

        }

        public void DisplayAllStackCards(int stackId)
        {

            using (var connection = new SqlConnection(MockDatabase.GetConnectionString()))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT Question, Answer FROM FlashCard WHERE StackId = @stackId";
                selectCmd.Parameters.AddWithValue("@stackId", stackId);

                int count = 1;
                using (var reader = selectCmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No Flash Cards found.");
                        return;
                    }

                    Console.WriteLine("Flash Cards:");
                    while (reader.Read())
                    {
                        // insert reader params to new FlashCard obkject
                        string question = reader.GetString(0);
                        string answer = reader.GetString(1);
                        Console.WriteLine($"id: {count} Question: {question} Answer: {answer}");
                        count += 1;


                    }
                }

                connection.Close();
            }

        }

        public void DisplayAllStacks()
        {

            using (var connection = new SqlConnection(MockDatabase.GetConnectionString()))
            {
                connection.Open();

                var selectCmd = connection.CreateCommand();
                selectCmd.CommandText = "SELECT Name FROM Stack";

                using (var reader = selectCmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("No stacks found.");
                        return;
                    }

                    Console.WriteLine("Stacks:");
                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        Console.WriteLine($"- {name}");
                        //MakeCardImage(name);
                    }
                }

                connection.Close();
            }
        }



    }
}

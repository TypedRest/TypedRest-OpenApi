﻿using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using TypedRest.OpenApi.Cli.Commands;

namespace TypedRest.OpenApi.Cli
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                return Parser.Default
                             .ParseArguments<Pattern, Generate>(args)
                             .MapResult(
                                  (Pattern command) => command.Run(),
                                  (Generate command) => command.Run(),
                                  _ => 1);
            }
            catch (IOException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
            catch (InvalidOperationException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
            catch (KeyNotFoundException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}

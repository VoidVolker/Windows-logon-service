//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace LogonService
//{
//    class Args : Dictionary<string, Args.Command>
//    {
//        public char[] prefixes = { '/', '-' };
//        public Action<string> msg = (s) => Console.WriteLine(s);
//        public struct Command
//        {
//            public bool isParsed;
//            public string usage;
//            public Func<string[], int, Action<string>, int> action;
//            public Command(Func<string[], int, Action<string>, int> a, string u = "")
//            {
//                isParsed = false;
//                action = a;
//                usage = u;
//            }
//        }

//        public string Usage()
//        {
//            string r = "";
//            foreach (KeyValuePair<string, Args.Command> cmd in this)
//            {
//                if (prefixes.Length > 0)
//                {
//                    r += "    ";
//                    //r + = prefixes.Join(cmd.Key, " ")
//                    foreach (char c in prefixes)
//                    {
//                        r += c + cmd.Key + " ";
//                    }
//                    r += "  ——  " + cmd.Value.usage + "\n";
//                }
//                else
//                {
//                    r += "    " + cmd.Key + "  ——  " + cmd.Value.usage + "\n";
//                }

//            }
//            return r;
//        }

//        public void Parse(string[] args)
//        {
//            int len = args.Length;
//            string argument;
//            //int cmdres = 0;
//            int i = 0;
//            while (i < len)
//            {
//                //msg("i= " + i + " len= " + len + (i < len).ToString());
//                Command cmd;
//                argument = args[i];
//                if (prefixes.Length > 0)
//                {
//                    if (prefixes.Contains(argument[0]))
//                    {
//                        argument = argument.Substring(1);
//                        if (TryGetValue(argument, out cmd))
//                        {
//                            if (cmd.isParsed)
//                            {
//                                msg("Argument already parsed: " + argument);
//                                return;
//                            }
//                            else
//                            {
//                                cmd.isParsed = true;
//                                this[argument] = cmd;
//                                i = cmd.action(args, i + 1, msg);
//                                continue;
//                            }

//                        }
//                    }
//                }
//                else
//                {
//                    if (TryGetValue(argument, out cmd))
//                    {
//                        if (cmd.isParsed)
//                        {
//                            msg("Argument already parsed: " + argument);
//                            return;
//                        }
//                        else
//                        {
//                            cmd.isParsed = true;
//                            this[argument] = cmd;
//                            i = cmd.action(args, i + 1, msg);
//                            continue;
//                        }
//                    }
//                }
//                msg("    Unknown argument: " + argument + "\n" + Usage());
//                return;
//            }
//        }

//        public Args()
//        {
//            Add("h", new Command((string[] a, int next, Action<string> m) =>
//            {
//                m(Usage());
//                return next;
//            }, " Show all commands help")
//            );
//        }
//    }
//}

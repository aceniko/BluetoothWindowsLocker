﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BluetoothWindowsLocker.Command
{
    /// <summary>
    /// class allowing to parse command line arguments
    /// copied as-is from
    /// <see cref="http://www.codeproject.com/KB/recipes/command_line.aspx"/>
    /// </summary>
    public class Arguments
    {
        /// <summary>
        /// parameters dictionary
        /// </summary>
        private readonly StringDictionary m_parameters;

        /// <summary>
        /// default constructor
        /// </summary>
        ///
        /// <param name="args">
        /// array of command line arguments
        /// </param>
        public Arguments(IEnumerable<string> args)
        {
            var spliter = new Regex(@"^-{1,2}|^/|=|:",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var remover = new Regex(@"^['""]?(.*?)['""]?$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            string parameter = null;

            string[] parts;

            m_parameters = new StringDictionary();

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((",')value(",'))
            // Examples: 
            // -param1 value1 --param2 /param3:"Test-:-work" 
            //   /param4=happy -param5 '--=nice=--'
            foreach (string txt in args)
            {
                // Look for new parameters (-,/ or --) and a
                // possible enclosed value (=,:)
                parts = spliter.Split(txt, 3);

                switch (parts.Length)
                {
                    // Found a value (for the last parameter 
                    // found (space separator))
                    case 1:
                        if (parameter != null)
                        {
                            if (!m_parameters.ContainsKey(parameter))
                            {
                                parts[0] =
                                    remover.Replace(parts[0], "$1");

                                m_parameters.Add(parameter, parts[0]);
                            }
                            parameter = null;
                        }
                        // else Error: no parameter waiting for a value (
                        // skipped)
                        break;
                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!m_parameters.ContainsKey(parameter))
                            {
                                m_parameters.Add(parameter, "true");
                            }
                        }
                        parameter = parts[1];
                        break;
                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. 
                        // With no value, set it to true.
                        if (parameter != null)
                        {
                            if (!m_parameters.ContainsKey(parameter))
                            {
                                m_parameters.Add(parameter, "true");
                            }
                        }

                        parameter = parts[1];

                        // Remove possible enclosing characters (",')
                        if (!m_parameters.ContainsKey(parameter))
                        {
                            parts[2] = remover.Replace(parts[2], "$1");
                            m_parameters.Add(parameter, parts[2]);
                        }

                        parameter = null;
                        break;
                }
            }

            // In case a parameter is still waiting
            if (parameter != null)
            {
                if (!m_parameters.ContainsKey(parameter))
                {
                    m_parameters.Add(parameter, "true");
                }
            }
        }

        /// <summary>
        /// Retrieve a parameter value if it exists (overriding C# indexer
        ///  property)
        /// </summary>
        ///
        /// <param name="param">
        /// name of required parameter
        /// </param>
        ///
        /// <returns>
        /// parameter value
        /// </returns>
        public string this[string param]
        {
            get
            {
                return (m_parameters[param]);
            }
        }
    }
}

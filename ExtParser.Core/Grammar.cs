using System.Collections.Generic;
using System.Linq;

namespace ExtParser.Core
{
    /// <summary>
    /// Basic implementation of the parser grammar.
    /// </summary>
    /// <typeparam name="TToken">Type of tokens this grammar is applicable to.</typeparam>
    public sealed class Grammar<TToken> : IGrammar<TToken>
    {
        /// <summary>
        /// Global parser rules.
        /// </summary>
        private readonly Dictionary<string, IParserRule<TToken>> globalRules;

        /// <summary>
        /// Synchronization object for global rules cache initialization.
        /// </summary>
        private readonly object globalRulesCacheSyncObj = new object();

        /// <summary>
        /// Cache of the global rules as array.
        /// </summary>
        private IParserRule<TToken>[] globalRulesCache;

        /// <summary>
        /// Regular grammar rules.
        /// </summary>
        private readonly Dictionary<string, IParserRule<TToken>> rules;

        /// <summary>
        /// Gets all global parser rules.
        /// </summary>
        public IParserRule<TToken>[] GlobalRules
        {
            get
            {
                var cachedValue = globalRulesCache;

                if (cachedValue == null)
                {
                    lock (globalRulesCacheSyncObj)
                    {
                        cachedValue = globalRulesCache;

                        if (cachedValue == null)
                        {
                            globalRulesCache = cachedValue =
                                globalRules.Values.ToArray();
                        }
                    }
                }

                return cachedValue;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Grammar{TToken}"/> class.
        /// </summary>
        public Grammar()
        {
            globalRules = new Dictionary<string, IParserRule<TToken>>();
            rules = new Dictionary<string, IParserRule<TToken>>();
        }

        /// <summary>
        /// Retrieves specified parser rule from the grammar.
        /// </summary>
        /// <param name="ruleName">Name of the parser rule</param>
        /// <returns>Requested parser rule.</returns>
        public IParserRule<TToken> GetRule(string ruleName)
        {
            IParserRule<TToken> rule;

            if (rules.TryGetValue(ruleName, out rule))
            {
                return rule;
            }

            throw new KeyNotFoundException("Rule " + ruleName + " is not defined");
        }

        /// <summary>
        /// Adds a parser rule to the grammar.
        /// </summary>
        /// <param name="rule">Instance of the parser rule implementation</param>
        /// <param name="isGlobal">Flag that indicates whether parser rule is global</param>
        public void AddRule(IParserRule<TToken> rule, bool isGlobal = false)
        {
            if (isGlobal)
            {
                globalRules[rule.RuleName] = rule;
                globalRulesCache = null;
            }
            else
            {
                rules[rule.RuleName] = rule;
            }
        }
    }
}

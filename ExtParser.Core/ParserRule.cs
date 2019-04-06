using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace ExtParser.Core
{
    /// <summary>
    /// Base class for all parser rules.
    /// </summary>
    /// <typeparam name="TToken">Type of the tokens this rule matches.</typeparam>
    public abstract class ParserRule<TToken> : IParserRule<TToken>
    {
        /// <summary>
        /// Gets the value indicating whether rule is atomic.
        /// </summary>
        /// <remarks>
        /// Atomic rules usually match single tokens that don't have any meaning without the context.
        /// Matches for such rules are not tracked to reduce the size of the parse tree.
        /// </remarks>
        protected virtual bool IsAtomicRule => false;

        /// <summary>
        /// Gets the name of the current rule.
        /// </summary>
        public abstract string RuleName { get; }

        /// <summary>
        /// Traces rule call and matches expected tokens from the stream.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>All possible parsing branches, if rule matches successfully, otherwise null.</returns>
        public async Task<IReadOnlyCollection<IParsingContext<TToken>>> Match(IParsingContext<TToken> context)
        {
            TraceDebug(context, "Enter: {0}", RuleName);

            // Prevent left recursion
            int currentRuleCount;
            if (context.ActiveRules.TryGetValue(this, out currentRuleCount)
                && currentRuleCount >= 2)
            {
                TraceDebug(context, "Terminating branch to prevent left recursion");
                return null;
            }

            // Increment rule entrances count
            context.ActiveRules[this] = currentRuleCount + 1;
            
            // Update parse tree, if needed
            ParseTreeNode parseTreeNode = null;

            if (!IsAtomicRule)
            {
                // Add new parse tree node and set is as current in the context
                parseTreeNode =
                    new ParseTreeNode(RuleName, context.TokenStream.Position, 0);

                if (context.ParseTree != null)
                {
                    context.ParseTree.AddChild(parseTreeNode);
                }
                
                context.ParseTree = parseTreeNode;
            }

            // Run matching logic
            var result =
                await MatchInternal(context);

            // Update parse tree for all resulting branches, as they may be cloned
            if (parseTreeNode != null)
            {
                if (result != null && result.Count > 0)
                {
                    // For each resulting branch unwind to parent
                    // Given that all children rules will do the same, current node in every context
                    // should be pointing to this rule, so we only need to adjust it to parent.
                    foreach (var branch in result)
                    {
                        branch.ParseTree.EndPosition = branch.TokenStream.Position;

                        if (parseTreeNode.Parent != null)
                        {
                            // If this rule was not an entry-point

                            if (branch.ParseTree.Parent == null)
                            {
                                // This is a cloned branch, there is nothing above this node
                                // Clone parent excluding self, so we can unwind before leaving the rule
                                var newParent =
                                    parseTreeNode.Parent.Clone(parseTreeNode);

                                newParent.AddChild(branch.ParseTree);
                            }

                            branch.ParseTree = branch.ParseTree.Parent;
                        }
                    }
                }
            }

            TraceDebug(context, "Exit ({1}): {0}", this, result == null ? 0 : result.Count);

            return result;
        }
        
        /// <summary>
        /// Matches expected tokens from the stream.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <returns>All possible parsing branches, if rule matches successfully, otherwise null.</returns>
        protected abstract Task<IReadOnlyCollection<IParsingContext<TToken>>> MatchInternal(IParsingContext<TToken> context);

        /// <summary>
        /// Writes a message to the trace.
        /// </summary>
        /// <param name="context">Parsing context</param>
        /// <param name="messageFormat">Message format</param>
        /// <param name="args">Format arguments</param>
        [Conditional("DEBUG")]
        protected virtual void TraceDebug(
            IParsingContext<TToken> context,
            string messageFormat,
            params object[] args)
        {
            var message = string.Format(CultureInfo.InvariantCulture, messageFormat, args);
            Debug.WriteLine("[" + context.BranchId + "] " + message);
        }

        /// <summary>
        /// Retrieves the name of this rule.
        /// </summary>
        /// <returns>Name of this rule.</returns>
        public override string ToString()
        {
            return GetType().Name;
        }
    }
}

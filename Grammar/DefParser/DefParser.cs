/*
 * DefParser.cs
 *
 * THIS FILE HAS BEEN GENERATED AUTOMATICALLY. DO NOT EDIT!
 */

using System.IO;

using PerCederberg.Grammatica.Runtime;

namespace InternalDSL {

    /**
     * <remarks>A token stream parser.</remarks>
     */
    internal class DefParser : RecursiveDescentParser {

        /**
         * <summary>An enumeration with the generated production node
         * identity constants.</summary>
         */
        private enum SynteticPatterns {
            SUBPRODUCTION_1 = 3001,
            SUBPRODUCTION_2 = 3002,
            SUBPRODUCTION_3 = 3003,
            SUBPRODUCTION_4 = 3004,
            SUBPRODUCTION_5 = 3005,
            SUBPRODUCTION_6 = 3006
        }

        /**
         * <summary>Creates a new parser with a default analyzer.</summary>
         *
         * <param name='input'>the input stream to read from</param>
         *
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        public DefParser(TextReader input)
            : base(input) {

            CreatePatterns();
        }

        /**
         * <summary>Creates a new parser.</summary>
         *
         * <param name='input'>the input stream to read from</param>
         *
         * <param name='analyzer'>the analyzer to parse with</param>
         *
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        public DefParser(TextReader input, DefAnalyzer analyzer)
            : base(input, analyzer) {

            CreatePatterns();
        }

        /**
         * <summary>Creates a new tokenizer for this parser. Can be overridden
         * by a subclass to provide a custom implementation.</summary>
         *
         * <param name='input'>the input stream to read from</param>
         *
         * <returns>the tokenizer created</returns>
         *
         * <exception cref='ParserCreationException'>if the tokenizer
         * couldn't be initialized correctly</exception>
         */
        protected override Tokenizer NewTokenizer(TextReader input) {
            return new DefTokenizer(input);
        }

        /**
         * <summary>Initializes the parser by creating all the production
         * patterns.</summary>
         *
         * <exception cref='ParserCreationException'>if the parser
         * couldn't be initialized correctly</exception>
         */
        private void CreatePatterns() {
            ProductionPattern             pattern;
            ProductionPatternAlternative  alt;

            pattern = new ProductionPattern((int) DefConstants.ROOT,
                                            "Root");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) DefConstants.LIST, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) DefConstants.OPERATOR,
                                            "Operator");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_1, 1, 1);
            alt.AddProduction((int) DefConstants.OP, 1, 1);
            alt.AddProduction((int) DefConstants.CONTEXT, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) DefConstants.CONTEXT,
                                            "Context");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.OPEN_TABLE, 1, 1);
            alt.AddProduction((int) DefConstants.LIST, 0, 1);
            alt.AddToken((int) DefConstants.CLOSE_TABLE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) DefConstants.ATOM, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.IDENTIFIER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) DefConstants.LIST,
                                            "List");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_4, 1, -1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) DefConstants.OP,
                                            "Op");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.EQUALS, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.MORE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.LESS, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) DefConstants.ATOM,
                                            "Atom");
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) DefConstants.DATE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) DefConstants.DECIMAL, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.TRUE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.FALSE, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.STRING, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) DefConstants.DECIMAL,
                                            "Decimal");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.NEGATIVE, 0, 1);
            alt.AddToken((int) DefConstants.NUMBER, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_6, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) DefConstants.DATE,
                                            "Date");
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.NUMBER, 1, 1);
            alt.AddToken((int) DefConstants.DOT, 1, 1);
            alt.AddToken((int) DefConstants.NUMBER, 1, 1);
            alt.AddToken((int) DefConstants.DOT, 1, 1);
            alt.AddToken((int) DefConstants.NUMBER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_1,
                                            "Subproduction1");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) DefConstants.ATOM, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.IDENTIFIER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_2,
                                            "Subproduction2");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) DefConstants.ATOM, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.IDENTIFIER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_3,
                                            "Subproduction3");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) DefConstants.OP, 1, 1);
            alt.AddProduction((int) DefConstants.CONTEXT, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_4,
                                            "Subproduction4");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_2, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_3, 0, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_5,
                                            "Subproduction5");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.NUMBEREND, 1, 1);
            pattern.AddAlternative(alt);
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.NUMBER, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);

            pattern = new ProductionPattern((int) SynteticPatterns.SUBPRODUCTION_6,
                                            "Subproduction6");
            pattern.Synthetic = true;
            alt = new ProductionPatternAlternative();
            alt.AddToken((int) DefConstants.DOT, 1, 1);
            alt.AddProduction((int) SynteticPatterns.SUBPRODUCTION_5, 1, 1);
            pattern.AddAlternative(alt);
            AddPattern(pattern);
        }
    }
}

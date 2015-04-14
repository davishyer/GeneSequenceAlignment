using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticsLab
{
    class PairWiseAlign
    {
        
        private int INDEL = 5;
        private int SUB = 1;
        private int MATCH = -3;

        private Char[] charA, charB;
        private int[] resultRow;

        private void initialize(GeneSequence sequenceA, GeneSequence sequenceB)
        {
            // grab first 5000 characters to evaluate
            charA = stringLimit(sequenceA.Sequence, 5000).ToCharArray();
            charB = stringLimit(sequenceB.Sequence, 5000).ToCharArray();
            resultRow = new int[charB.Length + 1];

            // initialize bottom row with costs for INDEL
            for (int i = 0; i < resultRow.Length; i++)
                resultRow[i] = i * INDEL;
        }

        public int Align(GeneSequence sequenceA, GeneSequence sequenceB, ResultTable resultTableSoFar, int rowInTable, int columnInTable)
        {
            // set up algorithm
            initialize(sequenceA, sequenceB);

            // calculate each additional row 
            for (int i = 0; i < charA.Length; i++)
                resultRow = computeNextRow(resultRow, charA[i], charB);

            // return score
            return resultRow[resultRow.Length - 1];
        }

        public String[] extraction(GeneSequence sequenceA, GeneSequence sequenceB)
        {
            // set up backtrace
            initialize(sequenceA, sequenceB);

            // initialize table to store each row
            List<int[]> resultTable = new List<int[]>(charA.Length + 1);
            resultTable.Add(resultRow);

            // calculate individual table
            for (int i = 0; i < charA.Length; i++)
                resultTable.Add(computeNextRow(resultTable[i], charA[i], charB));

            // initialize stringholders
            StringBuilder one = new StringBuilder();
            StringBuilder two = new StringBuilder();
            int row = charA.Length;
            int col = charB.Length;

            // backtrace strings
            while (row != 0 || col != 0)
            {
                if (resultTable[row][col] == resultTable[row][col - 1] + INDEL)
                {
                    one.Append('-');
                    two.Append(charB[--col]);
                }   
                else if (resultTable[row][col] == resultTable[row - 1][col] + INDEL)
                {
                    one.Append(charA[--row]);
                    two.Append('-');
                }               
                else if (resultTable[row][col] == resultTable[row - 1][col - 1] + MATCH ||
                    resultTable[row][col] == resultTable[row - 1][col - 1] + SUB)
                {
                    one.Append(charA[--row]);
                    two.Append(charB[--col]);
                
                }
                else
                    throw new ArgumentException();
            }

            String[] results = new String[2];
            results[0] = reverseString(one.ToString());
            results[1] = reverseString(two.ToString());
            return results;
        }

        //reverses string
        private String reverseString(String s)
        {
            Char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new String(charArray);
        }


        // uses previous row to compute next cell -> keeps memory in n space
        public int[] computeNextRow(int[] bottomRow, Char currentCharA, Char[] charB)
        {
            int[] topScores = new int[bottomRow.Length];
            topScores[0] = bottomRow[0] + INDEL; // each row[0] element goes up by the cost of an indel
            for (int j = 1; j < topScores.Length; j++)
            {
                // new score calc'd by grabbing min of the three edits
                topScores[j] = Math.Min(Math.Min(
                    topScores[j - 1] + INDEL, bottomRow[j] + INDEL),
                    currentCharA == charB[j - 1] ? bottomRow[j - 1] + MATCH : bottomRow[j - 1] + SUB);
            }
            return topScores;
        }

        // returns at most first 'limit' char of sequence
        public String stringLimit(String seq, int limit)
        {
            if (seq.Length > limit)
                return seq.Substring(0, limit);
            return seq;

        }
    }
}

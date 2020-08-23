using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PimpMyAvalonia.LanguageServer
{
    public static class Utils
    {
        public static int PositionToOffset(Position position, ReadOnlySpan<char> data)
        {
            return PositionToOffset(position.Line, position.Character, data);
        }

        public static int PositionToOffset(int line, int character, ReadOnlySpan<char> data)
        {
            int position = 0;
            for (int i = 0; i < line; i++)
            {
                position = FindNextLine(data, position);
            }
            position += character;
            return position;
        }

        private static int FindNextLine(ReadOnlySpan<char> data, int position)
        {
            while (position < data.Length)
            {
                if (data[position] == '\n')
                {
                    position++;
                    return position;
                }
                else if (data[position] == '\r')
                {
                    bool foundRN = false;
                    if (data.Length > position + 1)
                    {
                        if (data[position + 1] == '\n')
                        {
                            foundRN = true;
                        }
                    }

                    if (foundRN)
                    {
                        position += 2;
                        return position;
                    }
                    else
                    {
                        position += 1;
                        return position;
                    }
                }
                else
                {
                    position += 1;
                }
            }

            return position;
        }
    }
}

/* ***************************************************************************
 * This file is part of the Redzen code library.
 * 
 * Copyright 2015-2019 Colin Green (colin.green1@gmail.com)
 *
 * Redzen is free software; you can redistribute it and/or modify
 * it under the terms of The MIT License (MIT).
 *
 * You should have received a copy of the MIT License
 * along with Redzen; if not, see https://opensource.org/licenses/MIT.
 */

namespace Redzen
{
    /// <summary>
    /// General purpose helper methods.
    /// </summary>
    public static class VariableUtils
    {
        /// <summary>
        /// Swap two variables.
        /// </summary>
        /// <typeparam name="T">Variable type.</typeparam>
        /// <param name="a">First variable.</param>
        /// <param name="b">Second variable.</param>
        public static void Swap<T>(ref T a, ref T b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }
    }
}

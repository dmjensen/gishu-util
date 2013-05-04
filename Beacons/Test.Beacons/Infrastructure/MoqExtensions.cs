// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

﻿using System.Collections.Generic;
using System.Linq;
using Moq.Language;
using Moq.Language.Flow;

namespace ManiaX.Test.Beacons.Infrastructure
{
    static class MoqExtensions
    {
        public static IReturnsResult<TMock> ReturnsNextValueFrom<TMock,TResult>(this IReturns<TMock, TResult> returnValueEvaluator, IEnumerable<TResult> listOfResultValues)
            where TMock : class
        {
            var nextReturnValueIndex = 0;
            var returnsResult = returnValueEvaluator.Returns(() => listOfResultValues.ElementAt(nextReturnValueIndex));
            returnsResult.Callback(() => nextReturnValueIndex++);
            return returnsResult;
        }
    }
}
using System;

namespace PayPal.Forms.Abstractions
{
    public enum CreditCardType : long
    {
        Unrecognized,
        Ambiguous,
        Amex = 51L,
        Jcb = 74L,
        Visa = 52L,
        Mastercard,
        Discover
    }
}
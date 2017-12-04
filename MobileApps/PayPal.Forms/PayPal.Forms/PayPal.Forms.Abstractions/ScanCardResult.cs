using System;

namespace PayPal.Forms.Abstractions
{
    public class ScanCardResult
    {
        public PayPalStatus Status { get; }

        public Card Card { get; }

        public ScanCardResult(PayPalStatus status, Card card = null)
        {
            this.Status = status;
            this.Card = card;
        }
    }
}
using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName ="Validators/Integer")]
public class IntValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        // Allow digits and negative sign
        if (char.IsDigit(ch) || (ch == '-' && pos == 0 && !text.Contains("-")))
        {
            // Create the new text as if the character was added
            string newText = text.Insert(pos, ch.ToString());

            // Check if the new text is a valid integer
            if (IsValidInteger(newText))
            {
                pos++; // Move the caret forward if valid\
                text = newText;
                return ch;
            }
        }

        // Reject invalid characters
        return '\0';
    }

    private bool IsValidInteger(string text)
    {
        if (string.IsNullOrEmpty(text) || text == "-")
        {
            // Allow empty text or just a negative sign temporarily
            return true;
        }

        // Try parsing the text as a 32-bit signed integer
        if (int.TryParse(text, out _))
        {
            return true;
        }

        return false;
    }
}

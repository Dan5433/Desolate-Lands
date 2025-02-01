using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName ="Validators/Integer")]
public class IntValidator : TMP_InputValidator
{
    public override char Validate(ref string text, ref int pos, char ch)
    {
        //allow digits and negative sign
        if (char.IsDigit(ch) || (ch == '-' && pos == 0 && !text.Contains("-")))
        {
            string newText = text.Insert(pos, ch.ToString());

            if (IsValidInteger(newText))
            {
                pos++;
                text = newText;
                return ch;
            }
        }

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

namespace blazor.utils;

public class Utils
{
    public static async Task<T?> ExBackoff<T>(Func<T?> operation, int maxRetries = 5, int initialDelay = 1)
    {
        await Task.Delay(initialDelay);

        for (int i = 0; i < maxRetries; i++)
        {
            int round = (int)Math.Ceiling(Math.Pow(2, i));
            await Task.Delay(round);
            T? u = operation();
            if (u is not null)
            {
                return u;
            }
        }
        return default; 
    }
}
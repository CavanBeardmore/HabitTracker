const errorCodeToMessageMap: Map<number, string> = new Map<number, string>([
    [400, "Could not find content."],
    [401, "You are unauthorized. Please sign in again."],
    [403, "Invalid credentials."],
    [404, "Not Found."],
    [409, "Content already exists. Please refresh."],
    [429, "Too many requests. Please wait 5 minutes."]
])

export const errorCodeMapper = (errorCode: number) => {
    const message: string | undefined = errorCodeToMessageMap.get(errorCode);
    return message ? message : "An error has occurred.";
}
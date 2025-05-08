export interface IServerEventHandler {
    CloseConnection(): void
    CreateConnection(): Promise<void>
}
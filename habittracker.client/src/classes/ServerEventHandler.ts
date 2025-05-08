import { Resolve } from "@here-mobility/micro-di";
import { EventObserver } from "./EventObserver";
import { GlobalEventObserver } from "./GlobalEventObserver";
import { IServerEventHandler } from "./IServerEventHandler";

export enum EventType {
    USER_CREATED = "USER_CREATED",
    USER_UPDATED = "USER_UPDATED", 
    USER_DELETED = "USER_DELETED",
    LOGGED_IN = "LOGGED_IN",
    HABIT_ADDED = "HABIT_ADDED",
    HABIT_DELETED = "HABIT_DELETED",
    HABIT_UPDATED = "HABIT_UPDATED",
    HABIT_LOG_ADDED = "HABIT_LOG_ADDED",
    HABIT_LOG_UPDATED = "HABIT_LOG_UPDATED",
    HABIT_LOG_DELETED = "HABIT_LOG_DELETED",
    ERROR = "ERROR"
}

interface ServerEvent {
    EventType: EventType;
    Data?: unknown
} 

export class ServerEventHandler implements IServerEventHandler {
    private readonly _eventObserver: EventObserver = Resolve<GlobalEventObserver>(GlobalEventObserver);
    private _eventSource?: EventSource;

    constructor(private readonly _backendUrl: string) {}

    public async CreateConnection(): Promise<void> {
        const jwt = this.RetrieveJwtFromStorage();
        this._eventSource = new EventSource(`${this._backendUrl}events/Event/stream?token=${jwt}`);

        this._eventSource.onmessage = (event: any) => this.OnMessage(event);
        this._eventSource.onerror = (event: any) => this.OnError(event);
        this._eventSource.onopen = () => {
            console.log("Connection opened.");
        };
    }

    public CloseConnection(): void {
        this._eventSource?.close();
    }

    private OnMessage(e: MessageEvent): void {
        console.log("ServerEventHandler - OnMessage - received event", e);
        const event: ServerEvent = JSON.parse(e.data);
        const {EventType, Data} = event;
        console.log(`ServerEventHandler - OnMessage - raising event of type ${EventType} with data ${JSON.stringify(Data)}`);
        this._eventObserver.raise(EventType, Data);
    }

    private OnError(e: any): void {
        console.error("ServerEventHandler - OnError - received error", e)
        this._eventObserver.raise(EventType.ERROR);
    }

    private RetrieveJwtFromStorage(): string {
        const token = localStorage.getItem("AUTH_TOKEN");

        if (token === null) {
            throw new Error("No token found");
        }

        return token;
    }
}
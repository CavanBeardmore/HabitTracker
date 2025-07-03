import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { HabitLogService } from "../../src/classes/HabitLogService";
import { HabitLog } from "../../src/data/HabitLog";

describe("HabitLogService", () => {
    const fetchMock = vi.fn();
    const storageSetMock = vi.fn();
    const storageGetMock = vi.fn();
    const storageRemoveMock = vi.fn();

    beforeEach(() => {
        global.fetch = fetchMock;
        global.localStorage ={
            setItem: storageSetMock,
            getItem: storageGetMock,
            removeItem: storageRemoveMock
        } as Partial<Storage> as Storage
    });

    afterEach(() => {
        vi.restoreAllMocks();
    });

    describe("AddHabitLog", () => {
        it("should call fetch with correct arguments", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            vi.mocked(fetchMock).mockResolvedValue({
                ok: true,
                status: 200,
                headers: {
                get: (key: string) =>
                    key.toLowerCase() === "content-type" ? "application/json" : null,
                },
                json: async () => ({}),
            });

            const service = new HabitLogService("http://test-url/");

            await service.AddHabitLog(1);

            const expectedHeaders = new Headers();
            expectedHeaders.append("Authorization", "Bearer test");
            expectedHeaders.append("Content-Type", "application/json");

            expect(fetchMock).toHaveBeenCalledTimes(1);

            const [url, options] = fetchMock.mock.calls[0];

            expect(url).toBeInstanceOf(URL);
            expect((url as URL).href).toBe("http://test-url/habitLogs/HabitLog");
            expect(options.method).toBe("POST");
            expect(options.body).toBe('{"habit_id":1,"start_date":"2025-07-03","habit_logged":true,"length_in_days":1}');

            expect(options.headers).toBeInstanceOf(Headers);
            expect(options.headers.get("Authorization")).toBe("Bearer test");
            expect(options.headers.get("Content-Type")).toBe("application/json");
        })
    })

    describe("GetHabitLogs", () => {
        it("should return service response with array of habit logs when successful", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();
            headers.set("Content-Type", "application/json");

            const jsonData: HabitLog[] = [
                {
                    Id: 1,
                    Habit_id: 1,
                    Habit_logged: true,
                    LengthInDays: 1,
                    Start_date: "test-date"
                }
            ];
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 200,
                ok: true,
                json:() => Promise.resolve(jsonData)
            });

            const service = new HabitLogService("http://test-url/");

            const {success, status, data, errorMessage} = await service.GetHabitLogs(1, 2);

            expect(success).toBe(true);
            expect(status).toBe(200);
            expect(errorMessage).toBe(undefined);
            expect(data).toStrictEqual(jsonData);
        })

        it("should return service response with array of habit logs when successful", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();
            
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 500,
                statusText: "test",
                ok: false,
            });

            const service = new HabitLogService("http://test-url/");

            const {success, status, data, errorMessage} = await service.GetHabitLogs(1, 2);

            expect(success).toBe(false);
            expect(status).toBe(500);
            expect(errorMessage).toBe("test");
            expect(data).toBe(null);
        })
    })

    describe("GetMostRecentHabitLog", () => {
        it("should return service response with a habit log when successful", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();
            headers.set("Content-Type", "application/json");

            const jsonData: HabitLog = 
            {
                Id: 1,
                Habit_id: 1,
                Habit_logged: true,
                LengthInDays: 1,
                Start_date: "test-date"
            };

            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 200,
                ok: true,
                json:() => Promise.resolve(jsonData)
            });

            const service = new HabitLogService("http://test-url/");

            const {success, status, data, errorMessage} = await service.GetMostRecentHabitLog(1);

            expect(success).toBe(true);
            expect(status).toBe(200);
            expect(errorMessage).toBe(undefined);
            expect(data).toStrictEqual(jsonData);
        })

        it("should return service response with array of habit logs when successful", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();
            
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 500,
                statusText: "test",
                ok: false,
            });

            const service = new HabitLogService("http://test-url/");

            const {success, status, data, errorMessage} = await service.GetMostRecentHabitLog(1);

            expect(success).toBe(false);
            expect(status).toBe(500);
            expect(errorMessage).toBe("test");
            expect(data).toBe(null);
        })
    })

    describe("GetHabitLogById", () => {
        it("should return service response with a habit log when successful", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();
            headers.set("Content-Type", "application/json");

            const jsonData: HabitLog = 
            {
                Id: 1,
                Habit_id: 1,
                Habit_logged: true,
                LengthInDays: 1,
                Start_date: "test-date"
            };

            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 200,
                ok: true,
                json:() => Promise.resolve(jsonData)
            });

            const service = new HabitLogService("http://test-url/");

            const {success, status, data, errorMessage} = await service.GetHabitLogById(1);

            expect(success).toBe(true);
            expect(status).toBe(200);
            expect(errorMessage).toBe(undefined);
            expect(data).toStrictEqual(jsonData);
        })

        it("should return service response with array of habit logs when successful", async () => {
            vi.mocked(storageGetMock.mockReturnValue("test"));

            const headers = new Headers();
            
            vi.mocked(fetchMock).mockResolvedValue({
                headers,
                status: 500,
                statusText: "test",
                ok: false,
            });

            const service = new HabitLogService("http://test-url/");

            const {success, status, data, errorMessage} = await service.GetHabitLogById(1);

            expect(success).toBe(false);
            expect(status).toBe(500);
            expect(errorMessage).toBe("test");
            expect(data).toBe(null);
        })
    })
})
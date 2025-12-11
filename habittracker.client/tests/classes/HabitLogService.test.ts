import { describe, expect, it, vi } from "vitest";
import { HttpServiceRes, IHttpService, RequestMethod } from "../../src/classes/IHttpService";
import { HabitLogService } from "../../src/classes/HabitLogService";
import { before, beforeEach } from "node:test";
import { getToday } from "../../src/utils/getToday";

const Request = vi.fn();

const mockHttpService = {
    Request
} as IHttpService;

const habitLogService = new HabitLogService(
    "/testurl",
    mockHttpService,
);

describe("HabitLogService", () => {
    beforeEach(() => {
        vi.clearAllMocks();
    })

    describe("CreateHabitLog", () => {
        it("should call Request from http service with correct args", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            const id = 1;

            await habitLogService.CreateHabitLog(id);

            expect(Request).toBeCalledWith(
                "/testurl/HabitLog",
                {
                    method: RequestMethod.POST,
                    headers: [
                        {
                            key: "Content-Type", 
                            value: "application/json"
                        }
                    ],
                    body: JSON.stringify({
                        habit_id: 1,
                        start_date: getToday(),
                        habit_logged: true,
                        length_in_days: 1
                    })
                }
            );
        })
    })

    describe("GetHabitLogs", () => {
        it("should call Request from http service with correct args", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            const id = 1;
            const page = 1;

            await habitLogService.GetHabitLogs(id, page);

            expect(Request).toBeCalledWith(
                "/testurl/HabitLog/habit/1",
                {
                method: RequestMethod.GET,
                headers: [
                    {
                        key: "Content-Type", 
                        value: "application/json"
                    }
                ],
                params: [
                    {
                        key: "pageNumber",
                        value: 1
                    }
                ]
                }
            );
        })

        it("should return result from http service request", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            const id = 1;
            const page = 1;

            const result = await habitLogService.GetHabitLogs(id, page);

            expect(result).toStrictEqual(data);
        })
    })

    describe("GetMostRecentHabitLog", () => {
        it("should call Request from http service with correct args", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            const id = 1;

            await habitLogService.GetMostRecentHabitLog(id);

            expect(Request).toBeCalledWith(
                "/testurl/HabitLog/habit/recent/1",
                {
                    method: RequestMethod.GET,
                    headers: [
                        {
                            key: "Content-Type", 
                            value: "application/json"
                        }
                    ],
                }
            );
        })

        it("should return result from http service request", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            const id = 1;

            const result = await habitLogService.GetMostRecentHabitLog(id);

            expect(result).toStrictEqual(data);
        })
    })

    describe("GetHabitLogById", () => {
        it("should call Request from http service with correct args", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            const id = 1;

            await habitLogService.GetHabitLogById(id);

            expect(Request).toBeCalledWith(
                "/testurl/HabitLog/1",
                {
                    method: RequestMethod.GET,
                    headers: [
                        {
                            key: "Content-Type", 
                            value: "application/json"
                        }
                    ],
                }
            );
        })

        it("should return result from http service request", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            Request.mockResolvedValue(data);

            const id = 1;

            const result = await habitLogService.GetHabitLogById(id);

            expect(result).toStrictEqual(data);
        })
    })
})
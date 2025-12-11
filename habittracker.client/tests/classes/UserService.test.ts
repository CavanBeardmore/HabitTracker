import { describe, expect, it, vi } from "vitest";
import { UpdateUserArgs, UserService } from "../../src/classes/UserService";
import { User } from "../../src/data/User";
import { beforeEach } from "node:test";
import { HttpServiceRes, IHttpService, RequestMethod } from "../../src/classes/IHttpService";
import { Credentials } from "../../src/classes/AuthenticationService";

const authedRequest = vi.fn();
const nonAuthedRequest = vi.fn();

const mockHttpService = {
    Request: nonAuthedRequest
} as IHttpService;

const mockAuthHttpService = {
    Request: authedRequest
} as IHttpService;

const userService = new UserService(
    "/testurl",
    mockHttpService,
    mockAuthHttpService
);

describe("UserService", () => {
    beforeEach(() => {
        vi.clearAllMocks();
    })

    describe("GetUser", () => {
        it("should call http service Request with expected args", async () => {
            const data: HttpServiceRes<User> = {
                success: true,
                status: 200,
                data: {
                    Email: "test-email",
                    Password: "test-password",
                    Username: "test-username"
                } as User
            };

            nonAuthedRequest.mockResolvedValue(data);

            await userService.GetUser();

            expect(authedRequest).toBeCalledWith(
                "/testurl/User",
                {
                    method: RequestMethod.GET,
                }
            );
        });

        it("should return httpServiceRes from http service", async () => {
            const data: HttpServiceRes<User> = {
                success: true,
                status: 200,
                data: {
                    Email: "test-email",
                    Password: "test-password",
                    Username: "test-username"
                } as User
            };

            authedRequest.mockResolvedValue(data);

            const result = await userService.GetUser();

            expect(result).toStrictEqual(data);
        });
    })

    describe("CreateUser", () => {
        it("should call http service request with correct args", async () => {
            const data: HttpServiceRes<{StatusCode?: number, Message?: string}> = {
                success: true,
                status: 200,
                data: null
            };

            nonAuthedRequest.mockResolvedValue(data);

            const creds = {
                email: "test-email",
                username: "test-username",
                password: "test-password"
            };

            await userService.CreateUser(creds);

            expect(nonAuthedRequest).toBeCalledWith(
                "/testurl/User",
                {
                    method: RequestMethod.POST,
                    headers: [{key: "Content-Type", value: "application/json"}],
                    body: JSON.stringify(creds)
                }
            );
        });

        it("should return success as true with no error message when request succeeds", async () => {
            const data: HttpServiceRes<{StatusCode?: number, Message?: string}> = {
                success: true,
                status: 200,
                data: null
            };

            nonAuthedRequest.mockResolvedValue(data);

            const [success, message] = await userService.CreateUser({
                email: "test-email",
                username: "test-username",
                password: "test-password"
            });

            expect(success).toBe(true);
            expect(message).toBe("");
        });

        it("should return success as false with default error when data has no Message", async () => {
            const data: HttpServiceRes<{StatusCode?: number, Message?: string}> = {
                success: false,
                status: 404,
                data: null
            };

            nonAuthedRequest.mockResolvedValue(data);

            const [success, message] = await userService.CreateUser({
                email: "test-email",
                username: "test-username",
                password: "test-password"
            });

            expect(success).toBe(false);
            expect(message).toBe("An error has occurred.");
        });

        it("should return success as false with default error when data has no Message", async () => {
            const data: HttpServiceRes<{StatusCode?: number, Message?: string}> = {
                success: false,
                status: 404,
                data: {
                    StatusCode: 404,
                    Message: "error"
                }
            };

            nonAuthedRequest.mockResolvedValue(data);

            const [success, message] = await userService.CreateUser({
                email: "test-email",
                username: "test-username",
                password: "test-password"
            });

            expect(success).toBe(false);
            expect(message).toBe("error");
        });
    })

    describe("UpdateUser", () => {
        it("should call authed http service Request with correct args", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            authedRequest.mockResolvedValue(data);

            const args: UpdateUserArgs = {
                Email: "test-email",
                NewPassword: "test-new-pw",
                OldPassword: "test-old-pw",
                NewUsername: "test-new-un"
            };

            await userService.UpdateUser(args);

            expect(authedRequest).toBeCalledWith(
                "/testurl/User",
                {
                    method: RequestMethod.PATCH,
                    headers: [{key: "Content-Type", value: "application/json"}],
                    body: JSON.stringify(args)
                }
            );
        });
    })

    describe("DeleteUser", () => {
        it("should call authed http service Request with correct args", async () => {
            const data: HttpServiceRes = {
                success: true,
                status: 200,
                data: null
            };

            authedRequest.mockResolvedValue(data);

            const creds: Credentials = {
                username: "test-email",
                password: "test-pw",
            };

            await userService.DeleteUser(creds);

            expect(authedRequest).toBeCalledWith(
                "/testurl/User",
                {
                    method: RequestMethod.DELETE,
                    headers: [{key: "Content-Type", value: "application/json"}],
                    body: JSON.stringify(creds)
                }
            );
        });
    })
})
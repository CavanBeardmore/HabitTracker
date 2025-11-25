export interface Header {
    key: string,
    value: string
}

export interface Param {
    key: string,
    value: any
}

export enum RequestMethod {
    GET = "GET",
    PATCH = "PATCH",
    POST = "POST",
    DELETE = "DELETE"
} 

export interface httpOptions {
    method: RequestMethod
    headers?: Header[],
    params?: Param[]
    body?: string,
}

export interface HttpServiceRes<T = null> {
    success: boolean;
    status: number;
    data: T | null
    errorMessage?: string;
}

export interface IHttpService {
    Request<T>(url: string, options: httpOptions): Promise<HttpServiceRes<T>>
}
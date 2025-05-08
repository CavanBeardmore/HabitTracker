interface Header {
    key: string,
    value: string
}

interface Param {
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

export abstract class HttpService {

    public async Request<T>(url: string, options: httpOptions): Promise<HttpServiceRes<T>> {

        const {
            method,
            params,
            headers,
            body
        } = options;

        const requestUrl = this.BuildUrl(url, params || []);
        const requestHeaders = this.BuildHeaders(headers || []);

        const res = await fetch(
            requestUrl,
            {
                method,
                headers: requestHeaders,
                body
            }
        );

        console.log(res)

        const contentType = res.headers.get("Content-Type");
        const data = contentType?.includes("application/json")
            ? await res.json() 
            : null;

        const errorMessage = res.ok ? undefined : await res.statusText; 

        return {
            success: res.ok,
            status: res.status,
            data,
            errorMessage
        }
    }

    private BuildUrl(url: string, params: Param[]): URL {
        console.log(url)
        const requestUrl = new URL(url);

        if (params.length > 0) {
            params.forEach(v => requestUrl.searchParams.append(v.key, v.value));
        }

        return requestUrl;
    }

    private BuildHeaders(headers: Header[]): Headers {
        const reqHeaders = new Headers();

        for (const header of headers) {
            reqHeaders.append(header.key, header.value);
        }

        return reqHeaders;
    }
}
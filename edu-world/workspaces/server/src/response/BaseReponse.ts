export interface BaseResponseInterface<T> {
    meta: BaseMetaResponseInterface;
    data?: T
}

export interface BaseMetaResponseInterface {
    success: boolean;
    internalMessage: string;
    externalMessage: string;
    status: number;
}

export interface IPaginate<T> {
    page: number;
    pageSize: number;
    total: number;
    datas: T[]
}

export function successResponse<T>(data?: T): BaseResponseInterface<T> {
    const responseData: BaseResponseInterface<T> = {
        meta: {
            status: 200,
            success: true,
            externalMessage: "Success",
            internalMessage: "Success",
        }
    };

    if (data) {
        responseData.data = data;
    }

    return responseData;
}
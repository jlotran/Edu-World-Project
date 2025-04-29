/* eslint-disable @typescript-eslint/no-explicit-any */

/* Status code
    3000: Lỗi invalid data,
    3001: Lỗi dữ liệu
*/
class ValidationError extends Error {
    public statusCode: any;
    public data: any;

    constructor(message: string, status?: number, data?: any) {
        super(message);
        this.statusCode = status || 400;
        this.data = data || [];
    }
}
export default ValidationError;
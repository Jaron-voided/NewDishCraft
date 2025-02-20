import { Message } from "semantic-ui-react";
import {useMemo} from "react";

interface Props {
    errors: string[] | string | Error | null | undefined;
}

export default function ValidationErrors({ errors }: Props) {
    // Convert errors to an array of strings
    const errorArray = useMemo(() => {
        if (Array.isArray(errors)) {
            return errors.map(err => err.toString());
        } else if (errors instanceof Error) {
            return [errors.message];
        } else if (typeof errors === 'string') {
            return [errors];
        }
        return [];
    }, [errors]);

    if (errorArray.length === 0) {
        return null;
    }

    return (
        <Message error>
            <Message.List>
                {errorArray.map((err: string, i) => (
                    <Message.Item key={i}>{err}</Message.Item>
                ))}
            </Message.List>
        </Message>
    );
}
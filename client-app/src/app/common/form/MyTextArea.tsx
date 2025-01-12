import { useField } from "formik";
import { Form, Label } from "semantic-ui-react";
import { useState } from "react";

interface Props {
    placeholder: string;
    name: string;
    rows?: number; // Optional for initial height
    label?: string;
}

export default function MyTextArea(props: Props) {
    const [field, meta] = useField(props.name);
    const [textareaRows, setTextareaRows] = useState(props.rows || 3); // Default to 3 rows if not provided

    const handleInput = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
        const lineCount = event.target.value.split("\n").length; // Count lines
        setTextareaRows(lineCount < 3 ? 3 : lineCount); // Minimum 3 rows
    };

    return (
        <Form.Field error={meta.touched && !!meta.error}>
            <label>{props.label}</label>
            <textarea
                {...field}
                {...props}
                rows={textareaRows}
                onInput={handleInput} // Dynamically adjust rows
            />
            {meta.touched && meta.error ? (
                <Label basic color="red">{meta.error}</Label>
            ) : null}
        </Form.Field>
    );
}

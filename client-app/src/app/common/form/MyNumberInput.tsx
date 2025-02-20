import { useField } from "formik";
import { Form, Label } from "semantic-ui-react";
import {ChangeEvent} from "react";

interface Props {
    placeholder: string;
    name: string;
    label?: string;
    min?: number; // Optional: specify the minimum value
    max?: number; // Optional: specify the maximum value
    step?: number; // Optional: specify step increments
    value?: number;
    onChange?: (e: ChangeEvent<HTMLInputElement>) => void;
}

export default function MyNumberInput(props: Props) {
    const [field, meta] = useField(props.name);

    // Combine Formik's onChange with any custom onChange
    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        // First call Formik's built-in onChange
        field.onChange(e);

        // Then call any custom onChange if provided
        if (props.onChange) {
            props.onChange(e);
        }
    };

    return (
        <Form.Field error={meta.touched && !!meta.error}>
            <label>{props.label}</label>
            <input
                {...field}
                {...props}
                type="number" // Ensure the input is of type "number"
                onChange={handleChange}
            />
            {meta.touched && meta.error ? (
                <Label basic color="red">{meta.error}</Label>
            ) : null}
        </Form.Field>
    );
}

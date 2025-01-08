import { useField } from "formik";
import { Form, Label } from "semantic-ui-react";

interface Props {
    placeholder: string;
    name: string;
    label?: string;
    min?: number; // Optional: specify the minimum value
    max?: number; // Optional: specify the maximum value
    step?: number; // Optional: specify step increments
}

export default function MyNumberInput(props: Props) {
    const [field, meta] = useField(props.name);
    return (
        <Form.Field error={meta.touched && !!meta.error}>
            <label>{props.label}</label>
            <input
                {...field}
                {...props}
                type="number" // Ensure the input is of type "number"
            />
            {meta.touched && meta.error ? (
                <Label basic color="red">{meta.error}</Label>
            ) : null}
        </Form.Field>
    );
}

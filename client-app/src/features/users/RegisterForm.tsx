import {ErrorMessage, Form, Formik} from "formik";
import MyTextInput from "../../app/common/form/MyTextInput.tsx";
import {Button, Header} from "semantic-ui-react";
import {observer} from "mobx-react-lite";
import {useStore} from "../../app/stores/store.ts";
import * as Yup from "yup";
import ValidationErrors from "../errors/ValidationErrors.tsx";

export default observer(function RegisterForm() {
    const {userStore} = useStore();
    return (
        <Formik
            initialValues={{ displayName: '', userName: '', email: '', password: '', error: null}}
            onSubmit={(values, {setErrors}) => userStore.register(values).catch(error =>
                setErrors({error}))}
            validationSchema={Yup.object({
                displayName: Yup.string().required(),
                userName: Yup.string().required(),
                email: Yup.string().required(),
                password: Yup.string().required(),
            })}
        >
            {({handleSubmit, isSubmitting, errors, isValid, dirty}) => (
                <Form
                    className='ui form error'
                    onSubmit={handleSubmit}
                    autoComplete='off'
                >
                    <Header as='h2' content='Register to DishCraft' color='teal' textAlign='center'/>
                    <MyTextInput placeholder="Display Name" name="displayName" />
                    <MyTextInput placeholder="User Name" name="userName" />
                    <MyTextInput placeholder="Email" name="email" />
                    <MyTextInput placeholder="Password" name="password" type='password' />
                    <ErrorMessage
                        name='error'
                        render={() =>
                            <ValidationErrors errors={errors.error as unknown as string[]} />
                        }
                    />
                    <Button
                        disabled={!isValid || !dirty || isSubmitting }
                        loading={isSubmitting}
                        positive
                        content='Register'
                        type="submit"
                        fluid
                    />
                </Form>
            )}

        </Formik>
    )
})
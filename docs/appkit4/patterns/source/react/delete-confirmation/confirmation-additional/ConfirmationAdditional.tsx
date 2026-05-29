import React from 'react';
import { Button, Modal, Input } from '@appkit4/react-components';
import './index.scss';

const ConfirmationAdditional = (props: {solid?: boolean}) => {
    const [visible, setVisible] = React.useState(false);
    const [value, setValue] = React.useState('');
    
    const showModal = () => {
        setVisible(true);
    }

    const handleCancel = () => {
        setVisible(false);
    }

    const handleDelete = () => {
    }
    

    const handleChange = (value: string) => {
        setValue(value);
    }

    return (
        <>
            <Button onClick={showModal}>Delete report</Button>
            <Modal
                className='ap-pattern-confirmation'
                visible={visible}
                title='Delete report?'
                closable
                maskCloseable={false}
                onCancel={handleCancel}
                footer={
                    <>
                        <Button kind="secondary" onClick={handleCancel}>Cancel</Button>
                        <Button kind="negative" disabled={value !== 'Delete'} onClick={handleDelete}>Delete</Button>
                    </>
                }
            >
                <div className='ap-pattern-confirmation-message'>Are you sure you want to delete the current selection? This action is permanent.</div>
                <label className='ap-pattern-form-label'>
                    <span>Type “Delete” to confirm</span>
                    <Input
                        required
                        title="Type here"
                        value={value}
                        onChange={handleChange}
                    ></Input>
                </label>
            </Modal>
        </>
    );
}

export default ConfirmationAdditional;
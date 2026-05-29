import React from 'react';
import { Button, Modal } from '@appkit4/react-components';
import './index.scss';

const ConfirmationDefault = (props: {solid?: boolean}) => {
    const [visible, setVisible] = React.useState(false);
    
    const showModal = () => {
        setVisible(true);
    }

    const handleCancel = () => {
        setVisible(false);
    }

    const handleDelete = () => {
    }
    
    return (
        <>
            <Button onClick={showModal}>Delete report</Button>
            <Modal
                visible={visible}
                className='ap-pattern-confirmation'
                title='Delete report?'
                closable
                maskCloseable={false}
                onCancel={handleCancel}
                footer={
                    <>
                        <Button kind="secondary" onClick={handleCancel}>Cancel</Button>
                        <Button kind="negative" onClick={handleDelete}>Delete</Button>
                    </>
                }
            >
                <div className='ap-pattern-confirmation-message'>Are you sure you want to delete this report? This action is permanent.</div>
            </Modal>
        </>
    );
}

export default ConfirmationDefault;